using Gigras.Software.Database.Cyt.Entity.Models;
using System.Text;
using System.Xml;

namespace Gigras.Software.Generic.DynamicControl.Controls
{
    public static class TeatArea
    {
        public static string TeatAreaControl(FormField Field, string FieldType)
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

                if (FieldType == "textarea")
                {
                    htmlStringBuilder.AppendLine($@"
                    <div class='{Field.CssClass} mb-3'>
                        <label for='{fieldname}'>{Field.FieldType.FieldDescription}</label>
                        <textarea class='form-control' id='{fieldname}' name='{fieldname}'
                        {validationString}
                        />{Field.FieldValue ?? string.Empty}</textarea>
                        {validationErrorLabel}
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