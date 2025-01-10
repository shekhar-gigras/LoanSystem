using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.DynamicControl.Helper;
using Newtonsoft.Json;
using System.Text;

namespace Gigras.Software.Generic.DynamicControl.Controls
{
    public static class Checkbox
    {
        public static string CheckboxControl(FormField Field, string FieldType, CytContext context, int stateid)
        {
            var htmlStringBuilder = new StringBuilder();
            var fieldname = Field!.FieldType!.FieldName!.Replace(" ", "_");
            try
            {
                StringBuilder validationStringBuilder = new StringBuilder();

                foreach (var item in Field.FieldType!.FieldTypeValidations!.Select(x => x.FieldValidation))
                {
                    if (item!.ValidationType == "Required")
                    {
                        validationStringBuilder.Append("data-valid = 'required' ");
                    }
                    else if (item!.ValidationType == "Regex")
                    {
                        validationStringBuilder.Append($"data-regex='{item.ValidationValue}' ");
                    }
                }
                string validationString = validationStringBuilder.ToString().Trim();
                StringBuilder optionStringBuilder = new StringBuilder();
                if (Field.FieldType!.HasOptions)
                {
                    if (Field.FieldType!.FieldOption.IsDynamic)
                    {
                        string contextDbSet = Field.FieldType!.FieldOption.SourceTable.Replace("_", "");

                        var dbsettype = CommonHelper.GetDbSetType(contextDbSet.ToLower());
                        if (dbsettype != null)
                        {
                            var data = CommonHelper.GetDataDynamically(dbsettype, context, contextDbSet, Field.FieldType!.FieldOption.Condition);
                            if (data is IEnumerable<object> list)
                            {
                                foreach (var item in list)
                                {
                                    if (!string.IsNullOrEmpty(Field.FieldType!.FieldOption.TextValueField))
                                    {
                                        var fieldArray = Field.FieldType!.FieldOption.TextValueField.Split(',');
                                        var id = item.GetType().GetProperty(fieldArray[0]).GetValue(item);
                                        var name = item.GetType().GetProperty(fieldArray[1]).GetValue(item);
                                        var selected = "";
                                        if (Field.FieldValue != null)
                                        {
                                            if (Field.FieldValue.TrimStart().StartsWith("[") && Field.FieldValue.TrimEnd().EndsWith("]"))
                                            {
                                                // FieldValue is likely JSON, deserialize it
                                                var values = JsonConvert.DeserializeObject<List<string>>(Field.FieldValue);
                                                if (values != null && (values.Contains(id.ToString()) || values.Contains(name.ToString())))
                                                {
                                                    selected = "selected";
                                                }
                                            }
                                            else
                                            {
                                                // FieldValue is a simple string
                                                selected = Field.FieldValue == id.ToString() ? "selected" : "";
                                            }
                                        }
                                        optionStringBuilder.AppendLine($@"<div class='form-check'>");
                                        optionStringBuilder.AppendLine($@"<input type='{FieldType}' class='form-check-input' value='{id}' {validationString} id='{fieldname}' name='{fieldname}' {selected}>");
                                        optionStringBuilder.AppendLine($@"<label class='form-check-label' for='{fieldname}'>{name}</label>");
                                        optionStringBuilder.AppendLine($@"</div>");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var objOptions = context.DynamicFieldOptionValues.Where(cv => cv.OptionId == Field.FieldType!.FieldOption.Id);
                        if (objOptions != null && objOptions.Any())
                        {
                            foreach (var item in objOptions)
                            {
                                var selected = (Field.FieldValue != null ? (Field.FieldValue!.Contains(item.OptionValue!) ? "checked" : "") : "");
                                if (string.IsNullOrEmpty(selected) || (Field.FieldValue != null && (Field.FieldValue == "false" || Field.FieldValue == "true")))
                                {
                                    Field.FieldValue = (Field.FieldValue == "False" ? "0" : Field.FieldValue);
                                    Field.FieldValue = (Field.FieldValue == "True" ? "1" : Field.FieldValue);
                                    selected = (Field.FieldValue != null ? (Field.FieldValue!.Contains(item.OptionValue!) ? "checked" : "") : "");
                                }
                                optionStringBuilder.AppendLine($@"<div class='form-check'>");
                                optionStringBuilder.AppendLine($@"<input type='{FieldType}' class='form-check-input' value='{item.OptionValue}' {validationString} id='{fieldname}' name='{fieldname}' {selected}>");
                                optionStringBuilder.AppendLine($@"<label class='form-check-label' for={fieldname}'>{item.OptionLabel}</label>");
                                optionStringBuilder.AppendLine($@"</div>");
                            }
                        }
                    }
                }
                if (FieldType == "checkbox")
                {
                    htmlStringBuilder.AppendLine($@"
                    <div class='{Field.CssClass} mb-3'>
                        <label for='{Field.FieldType!.FieldName}' class='form-label' style='font-weight: 600;'>{Field.FieldType.FieldDescription}</label>
                            <div>
                                    <div class='form-check-container'>
                                        {optionStringBuilder}
                                    </div>
                            </div>
                    </div>
                    {(string.IsNullOrEmpty(Field.FieldType.JavaScript) ? string.Empty : $@"
                        <script>
                            {Field.FieldType.JavaScript}
                        </script>")}
                ");
                }
            }
            catch (Exception ex)
            {
                htmlStringBuilder = new StringBuilder();
                htmlStringBuilder.AppendLine($"<div class='error'>Error: {ex.Message}</div>");
            }

            return htmlStringBuilder.ToString();
        }
    }
}