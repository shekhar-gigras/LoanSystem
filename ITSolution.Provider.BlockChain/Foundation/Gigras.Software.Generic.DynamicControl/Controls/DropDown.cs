using Gigras.Software.Database;
using Gigras.Software.Database.Cyt.Entity.Models;
using Gigras.Software.Generic.DynamicControl.Helper;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Gigras.Software.Generic.DynamicControl.Controls
{
    public static class DropDown
    {
        public static string DropDownControl(FormField Field, string FieldType, CytContext context, int stateid)
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
                        validationStringBuilder.Append("required ");
                    }
                    else if (item!.ValidationType == "Regex")
                    {
                        validationStringBuilder.Append($"data-regex='{item.ValidationValue}' ");
                    }
                }
                string validationString = validationStringBuilder.ToString().Trim();
                StringBuilder optionStringBuilder = new StringBuilder();
                if (Field.FieldType!.FieldOption != null)
                {
                    if (Field.FieldType!.FieldOption.IsDynamic)
                    {
                        string contextDbSet = Field.FieldType!.FieldOption!.SourceTable!.Replace("_", "");

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
                                        optionStringBuilder.AppendLine($@"<option value='{id}' {selected}>{name}</option>");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var objOptions = context.DynamicFieldOptionValues.Where(cv => cv.OptionId == Field.FieldType!.FieldOption.Id && cv.IsActive && !cv.IsDelete);
                        if (objOptions != null && objOptions.Any())
                        {
                            foreach (var item in objOptions)
                            {
                                var selected = "";
                                if (!string.IsNullOrEmpty(Field.FieldValue))
                                {
                                    selected = Field.FieldValue!.Trim() == item.OptionValue!.Trim() ? "selected" : "";
                                }
                                optionStringBuilder.AppendLine($@"<option value='{item.OptionValue}' {selected}>{item.OptionLabel}</option>");
                            }
                        }
                    }
                }
                if (FieldType == "dropdown" || FieldType == "multidropdown")
                {
                    var multiple = (FieldType == "multidropdown" ? "multiple='multiple'" : "");
                    var multipleFielName = (FieldType == "multidropdown" ? fieldname + "[]" : fieldname);
                    htmlStringBuilder.AppendLine($@"
                    <div class='{Field.CssClass} mb-3'>
                        <label for='{fieldname}'>{Field.FieldType.FieldDescription}</label>
                        <select {multiple}  data-allow-clear=""true"" data-placeholder=""Select an {Field.FieldType.FieldDescription}"" class='form-select' id='{fieldname}' name='{multipleFielName}' data-control=""select2"" data-value='{Field.FieldValue}'
                        {validationString}
                        />
                            <option></option>
                            {optionStringBuilder}
                        </select>
                        {(string.IsNullOrEmpty(Field.FieldType.JavaScript) ? string.Empty : $@"
                            <script>
                                {Field.FieldType.JavaScript}
                           </script>")}
                        {(string.IsNullOrEmpty(Field.JavaScript) ? string.Empty : $@"
                            <script>
                                {Field.JavaScript}
                           </script>")}
                    </div>
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