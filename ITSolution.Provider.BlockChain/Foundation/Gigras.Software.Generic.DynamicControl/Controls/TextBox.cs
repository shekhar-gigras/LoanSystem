using Gigras.Software.Database.Cyt.Entity.Models;
using System.Text;

namespace Gigras.Software.Generic.DynamicControl.Controls
{
    public static class TextBox
    {
        public static string TextBoxControl(FormField Field, string FieldType)
        {
            var htmlStringBuilder = new StringBuilder();
            var fieldname = Field!.FieldType!.FieldName!.Replace(" ", "_");
            try
            {
                StringBuilder validationStringBuilder = new StringBuilder();
                StringBuilder validationErrorLabel = new StringBuilder();
                int i = 1;
                foreach (var item in Field.FieldType!.FieldTypeValidations!.Select(x => x.FieldValidation))
                {
                    if (!item!.IsDelete && item.IsActive)
                    {
                        string validationType = item!.ValidationType!.Replace(" ", "_");
                        if (item!.ValidationType == "Required")
                        {
                            validationStringBuilder.Append("required ");
                        }
                        else if (item!.ValidationType == "Regex")
                        {
                            validationStringBuilder.Append($@"data-regex='{item.ValidationValue}' ");
                        }
                        validationErrorLabel.AppendLine($"<span data-valmsg-for='{fieldname}' class='text-danger' style='display:none' id='{fieldname}-field-validation-valid-{validationType}-{i}' name='{fieldname}-field-validation-valid-{validationType}-{i}'>{item.ErrorMessage}</span>");
                        i++;
                    }
                }
                string validationString = validationStringBuilder.ToString().Trim();

                if (FieldType.ToLower() == "date" && !string.IsNullOrEmpty(Field.FieldValue))
                {
                    Field.FieldValue = (DateTime.TryParse(Field.FieldValue?.ToString(), out var date) ? date.ToString("yyyy-MM-dd") : string.Empty);
                }
                htmlStringBuilder.AppendLine($@"
                        <div class='{Field.CssClass} mb-3'
                                {(FieldType == "hidden" ? "style='display:none;'" : string.Empty)}
                            >
                            <label for='{fieldname}'>{Field.FieldType.FieldDescription}</label>
                            <input type='{FieldType}' class='form-control' id='{fieldname}' name='{fieldname}'
                            {validationString}
                             {(Field.FieldType.Steps.HasValue ? "step=" + Field.FieldType.Steps : string.Empty)}
                           {(Field.FieldType.MaxLength.HasValue ? "maxlength=" + Field.FieldType.MaxLength : string.Empty)}
                             {(Field.FieldType.MinValue.HasValue ? "minvalue=" + Field.FieldType.MinValue : string.Empty)}
                              {(Field.FieldType.MaxValue.HasValue ? "minvalue=" + Field.FieldType.MaxValue : string.Empty)}
                                 {(!string.IsNullOrEmpty(Field.FieldType.DefaultValue) ? "value=" + Field.FieldType.DefaultValue : string.Empty)}

                                 value='{Field.FieldValue ?? string.Empty}'
                            />
                            {validationErrorLabel}
                            {(string.IsNullOrEmpty(Field.JavaScript) ? string.Empty : $@"
                                <script>
                                    {Field.JavaScript}
                                </script>")}
                        </div>
                    ");
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