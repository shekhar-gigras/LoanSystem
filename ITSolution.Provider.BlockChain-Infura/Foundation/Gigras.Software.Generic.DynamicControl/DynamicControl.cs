using Gigras.Software.Database;
using Gigras.Software.Generic.DynamicControl.Controls;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace Gigras.Software.Generic.DynamicControl
{
    public static class DynamicControl
    {
        // Generate dynamic HTML control
        public static HtmlString DynamicHtmlControl<T>(this IHtmlHelper htmlHelper, T model, CytContext conext, int stateid)
        {
            var htmlStringBuilder = new StringBuilder();

            try
            {
                // Check if the model is of a specific type that needs dynamic rendering
                if (model is Database.Cyt.Entity.Models.FormField Field)
                {
                    switch (Field.FieldType!.CtrlType!.ToLower())
                    {
                        case "textbox":
                        case "email":
                        case "tel":
                        case "time":
                        case "url":
                        case "hidden":
                            htmlStringBuilder.AppendLine(TextBox.TextBoxControl(Field, Field.FieldType.CtrlType.ToLower()));
                            break;

                        case "date":
                        case "month":
                        case "week":
                            htmlStringBuilder.AppendLine(TextBox.TextBoxControl(Field, Field.FieldType.CtrlType.ToLower()));
                            break;

                        case "number":
                            htmlStringBuilder.AppendLine(TextBox.TextBoxControl(Field, Field.FieldType.CtrlType.ToLower()));
                            break;

                        case "textarea":
                            htmlStringBuilder.AppendLine(TeatArea.TeatAreaControl(Field, Field.FieldType.CtrlType.ToLower()));
                            break;

                        case "dropdown":
                            htmlStringBuilder.AppendLine(DropDown.DropDownControl(Field, "dropdown", conext, stateid));
                            break;

                        case "multidropdown":
                            htmlStringBuilder.AppendLine(DropDown.DropDownControl(Field, "multidropdown", conext, stateid));
                            break;

                        case "checkbox":
                            htmlStringBuilder.AppendLine(Checkbox.CheckboxControl(Field, Field.FieldType.CtrlType.ToLower(), conext, stateid));
                            break;

                        case "radio":
                            htmlStringBuilder.AppendLine(Radio.RadioControl(Field, Field.FieldType.CtrlType.ToLower(), conext, stateid));
                            break;
                    }
                    // Dynamically create a form control based on the model properties
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions gracefully
                htmlStringBuilder.AppendLine($"<div class='error'>Error: {ex.Message}</div>");
            }

            // Return the HTML string wrapped in an HtmlString
            return new HtmlString(htmlStringBuilder.ToString());
        }
    }
}