# YamlToJson task

Converts a YAML file to JSON format.

## Task parameters
The following table describes the parameters of the `YamlToJson` task.

|Parameter          | Description |
|-------------------|-------------|
|`YamlInputFile`  | Required `String` parameter<br/> The input YAML file name to convert. |
|`JsonOutputFile` | Required `String` parameter<br/> The output JSON file name.  |
|`WithIndentation`| Optional `Boolean` parameter<br/> Indicates if the JSON file to generate must indented. The default value is `true`. |

## Example

The following example uses the `YamlToJson` task to converts the `Api.yaml` YAML file
to `Api.json` JSON file with no indentation.

```xml
<Project>
  <Target Name="ConvertToJson">
    <YamlToJson YamlInputFile="Api.yaml" JsonOutputFile="Api.json" />
  </Target>
</Project>
```