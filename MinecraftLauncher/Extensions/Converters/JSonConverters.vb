Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json

#Region "JsonConverters"

Class CustomIntConverter
    Inherits JsonConverter
    Public Overrides Function CanConvert(objectType As Type) As Boolean
        Return (objectType = GetType(Integer))
    End Function

    Public Overrides Function ReadJson(reader As JsonReader, objectType As Type, existingValue As Object, serializer As JsonSerializer) As Object
        Dim jsonValue As JValue = serializer.Deserialize(Of JValue)(reader)

        If jsonValue.Type = JTokenType.Float Then
            Return CInt(Math.Round(CDbl(jsonValue.Value)))
        ElseIf jsonValue.Type = JTokenType.[Integer] Then
            Return CInt(jsonValue.Value)
        End If

        Throw New FormatException()
    End Function

    Public Overrides Sub WriteJson(writer As JsonWriter, value As Object, serializer As JsonSerializer)
        Throw New NotImplementedException()
    End Sub
End Class

#End Region