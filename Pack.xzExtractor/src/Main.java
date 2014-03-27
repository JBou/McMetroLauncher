import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Arrays;
import java.util.jar.JarEntry;
import java.util.jar.JarOutputStream;
import java.util.jar.Pack200;
import org.tukaani.xz.XZInputStream;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.nio.file.Path;

public class Main {

	public static void main(String[] args) throws IOException {
		if (args.length > 2) {
			System.out.println("Too many Arguments");
		} else if (args.length < 2) {
			System.out.println("Too little Arguments");
		} else if (!new File(args[0]).exists()) {
			System.out.println("InputFile does not exist");
		} else {
			try {
				File input = new File(args[0]);
				File output = new File(args[1]);
				Path path = Paths.get(input.getAbsolutePath());
				byte[] data = Files.readAllBytes(path);
				unpackLibrary(output, data);
				//success
				System.exit(0);
			} catch (Exception e) {
				System.out.println("Error unpacking File: " + e.getLocalizedMessage());
				//error
				System.exit(1);
			}
		}
	}

	public static void unpackLibrary(File output, byte[] data)
			throws IOException {
		if (output.exists()) {
			output.delete();
		}

		byte[] decompressed = readFully(new XZInputStream(
				new ByteArrayInputStream(data)));

		// Snag the checksum signature
		String end = new String(decompressed, decompressed.length - 4, 4);
		if (!end.equals("SIGN")) {
			System.out.println("Unpacking failed, signature missing " + end);
			return;
		}

		int x = decompressed.length;
		int len = ((decompressed[x - 8] & 0xFF))
				| ((decompressed[x - 7] & 0xFF) << 8)
				| ((decompressed[x - 6] & 0xFF) << 16)
				| ((decompressed[x - 5] & 0xFF) << 24);
		byte[] checksums = Arrays.copyOfRange(decompressed, decompressed.length
				- len - 8, decompressed.length - 8);

		FileOutputStream jarBytes = new FileOutputStream(output);
		JarOutputStream jos = new JarOutputStream(jarBytes);
		Pack200.newUnpacker().unpack(new ByteArrayInputStream(decompressed),
				jos);

		jos.putNextEntry(new JarEntry("checksums.sha1"));
		jos.write(checksums);
		jos.closeEntry();

		jos.close();
		jarBytes.close();
	}

	public static byte[] readFully(InputStream stream) throws IOException {
		byte[] data = new byte[4096];
		ByteArrayOutputStream entryBuffer = new ByteArrayOutputStream();
		int len;
		do {
			len = stream.read(data);
			if (len > 0) {
				entryBuffer.write(data, 0, len);
			}
		} while (len != -1);

		return entryBuffer.toByteArray();
	}

}
