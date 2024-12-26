using DevExpress.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Buffers.Text;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace UnseenOperator;

public partial class SimpleOperator : XtraForm
{
    public SimpleOperator()
    {
        InitializeComponent();
    }


    #region Helpers
    private void SetTabPage(XtraTabPage paramater)
    {
        DEP_UnseenOperator.ClearErrors();

        XTC_Types.SelectedTabPageIndex = XTC_Types.TabPages.IndexOf(paramater);
    }

    static string AddPaddingIfNeeded(string base64)
    {
        int remainder = base64.Length % 4;

        // Eksik padding kontrolü
        if (remainder == 2)
        {
            return base64 + "==";
        }
        else if (remainder == 3)
        {
            return base64 + "=";
        }
        else if (remainder == 0)
        {
            return base64; // Zaten doğru formatta
        }
        else
        {
            throw new FormatException("Geçersiz Base64 dizisi. Uzunluk Base64 için uygun değil.");
        }
    }

    private async void DoSaveOperation(bool isClipboard)
    {
        if (XTC_Types.TabPages.IndexOf(XTP_Base64) == XTC_Types.SelectedTabPageIndex && !string.IsNullOrEmpty(ME_Base64Output.Text))
        {
            if (isClipboard)
                Clipboard.SetText(ME_Base64Output.Text, TextDataFormat.UnicodeText);
            else
            {
                using SaveFileDialog saveFileDialog = new()
                {
                    AddExtension = true,
                    AddToRecent = true,
                    Filter = "Text File|*.txt",
                    DefaultExt = "txt",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                };

                if (DialogResult.OK == saveFileDialog.ShowDialog())
                {
                    FileStream stream = (FileStream)saveFileDialog.OpenFile();

                    await File.WriteAllBytesAsync(saveFileDialog.FileName, Encoding.UTF8.GetBytes(ME_Base64Output.Text)).ConfigureAwait(true);

                    stream.Close();

                    stream?.Dispose();
                }
            }
        }
        else if (XTC_Types.TabPages.IndexOf(XTP_Base64Image) == XTC_Types.SelectedTabPageIndex && PE_Base64ToImage.Image != null)
        {
            if (isClipboard)
                Clipboard.SetImage(PE_Base64ToImage.Image);
            else
            {
                using SaveFileDialog saveFileDialog = new()
                {
                    AddExtension = true,
                    AddToRecent = true,
                    Filter = "PNG|*.png|GIF|*.gif|BMP|*.bmp|JPEG|*.jpg;*.jpeg",
                    DefaultExt = "jpg",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                };

                if (DialogResult.OK == saveFileDialog.ShowDialog())
                {
                    FileStream stream = (FileStream)saveFileDialog.OpenFile();

                    PE_Base64ToImage.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                    stream.Close();

                    stream?.Dispose();
                }
            }
        }
        else if (XTC_Types.TabPages.IndexOf(XTP_ImageToBase64) == XTC_Types.SelectedTabPageIndex && !string.IsNullOrEmpty(ME_ImageToBase64.Text))
        {
            if (isClipboard)
                Clipboard.SetText(ME_ImageToBase64.Text);
            else
            {
                using SaveFileDialog saveFileDialog = new()
                {
                    AddExtension = true,
                    AddToRecent = true,
                    Filter = "Text File|*.txt",
                    DefaultExt = "txt",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                };

                if (DialogResult.OK == saveFileDialog.ShowDialog())
                {
                    FileStream stream = (FileStream)saveFileDialog.OpenFile();

                    await File.WriteAllBytesAsync(saveFileDialog.FileName, Encoding.UTF8.GetBytes(ME_ImageToBase64.Text)).ConfigureAwait(true);

                    stream.Close();

                    stream?.Dispose();
                }
            }
        }
        else if (XTC_Types.TabPages.IndexOf(XTP_Base64ToPdf) == XTC_Types.SelectedTabPageIndex && !string.IsNullOrEmpty(ME_Base64ToPdf.Text))
        {
            if (isClipboard)
                Clipboard.SetText(ME_Base64ToPdf.Text);
            else
            {
                using SaveFileDialog saveFileDialog = new()
                {
                    AddExtension = true,
                    AddToRecent = true,
                    Filter = "PDF File|*.pdf",
                    DefaultExt = "pdf",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                };

                if (DialogResult.OK == saveFileDialog.ShowDialog())
                {
                    FileStream stream = (FileStream)saveFileDialog.OpenFile();

                    PV_Base64ToPdf.SaveDocument(stream);

                    stream.Close();

                    stream?.Dispose();
                }
            }
        }
        else if (XTC_Types.TabPages.IndexOf(XTP_PdfToBase64) == XTC_Types.SelectedTabPageIndex && !string.IsNullOrEmpty(ME_PdfToBase64.Text))
        {
            if (isClipboard)
                Clipboard.SetText(ME_PdfToBase64.Text);
            else
            {
                using SaveFileDialog saveFileDialog = new()
                {
                    AddExtension = true,
                    AddToRecent = true,
                    Filter = "Text File|*.txt",
                    DefaultExt = "txt",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                };

                if (DialogResult.OK == saveFileDialog.ShowDialog())
                {
                    FileStream stream = (FileStream)saveFileDialog.OpenFile();

                    await File.WriteAllBytesAsync(saveFileDialog.FileName, Encoding.UTF8.GetBytes(ME_PdfToBase64.Text)).ConfigureAwait(true);

                    stream.Close();

                    stream?.Dispose();
                }
            }
        }
    }
    #endregion

    private void ACE_B64_Encode_Click(object sender, EventArgs e)
    {
        try
        {
            SetTabPage(XTP_Base64);
            if (!string.IsNullOrEmpty(ME_Base64Input.Text))
            {
                string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(ME_Base64Input.Text), Base64FormattingOptions.None);
                ME_Base64Output.EditValue = encoded;

                Clipboard.SetText(ME_Base64Output.Text);
            }
            else
                DEP_UnseenOperator.SetError(ME_Base64Input, "Please input valid base64 string to encode!");
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Please input valid string to encode! {ex.Message}", "SimpleOperator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }

    private void ACE_B64_Decode_Click(object sender, EventArgs e)
    {
        try
        {
            SetTabPage(XTP_Base64);
            if (!string.IsNullOrEmpty(ME_Base64Input.Text))
            {
                byte[] decoded = Convert.FromBase64String(AddPaddingIfNeeded(ME_Base64Input.Text));
                ME_Base64Output.EditValue = Encoding.UTF8.GetString(decoded);

                Clipboard.SetText(ME_Base64Output.Text);
            }
            else
                DEP_UnseenOperator.SetError(ME_Base64Input, "Please input valid base64 string to decode!");
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Please input valid base64 string to decode! {ex.Message}", "SimpleOperator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }


    private void ACE_CopyOutput_Click(object sender, EventArgs e)
    {
        DoSaveOperation(true);
    }

    private void ACE_SaveOutput_Click(object sender, EventArgs e)
    {
        DoSaveOperation(false);
    }


    private void ACE_B64_Image_Encode_Click(object sender, EventArgs e)
    {
        try
        {
            SetTabPage(XTP_ImageToBase64);

            if (PE_ImageToBase64.Image != null)
            {
                using MemoryStream stream = new();
                PE_ImageToBase64.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                ME_ImageToBase64.EditValue = Convert.ToBase64String(stream.ToArray());
            }
            else
                DEP_UnseenOperator.SetError(PE_ImageToBase64, "Please load valid image to convertable to base64!");
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Please select valid image to encode! {ex.Message}", "SimpleOperator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }

    private void ACE_B64_Image_Decode_Click(object sender, EventArgs e)
    {
        try
        {
            SetTabPage(XTP_Base64Image);

            if (!string.IsNullOrEmpty(ME_Base64ToImage.Text) && Base64.IsValid(ME_Base64ToImage.Text.AsSpan()))
            {

                PE_Base64ToImage.EditValue = DXImage.FromBase64String(ME_Base64ToImage.Text);
            }
            else
                DEP_UnseenOperator.SetError(ME_Base64ToImage, "Please input valid base64 for convertable to image!");
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Please input valid string to decode! {ex.Message}", "SimpleOperator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }


    private async void ACE_EncodePdf_Click(object sender, EventArgs e)
    {
        try
        {
            SetTabPage(XTP_PdfToBase64);

            using OpenFileDialog openFileDialog = new()
            {
                AddExtension = true,
                AddToRecent = true,
                DefaultExt = "pdf",
                Filter = "PDF File|*.pdf",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
            };

            openFileDialog.ShowDialog();

            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                var readedBytes = await File.ReadAllBytesAsync(openFileDialog.FileName).ConfigureAwait(true);

                ME_PdfToBase64.EditValue = Convert.ToBase64String(readedBytes, Base64FormattingOptions.None);
                PV_PdfToBase64.LoadDocument(new MemoryStream(readedBytes));
            }
            else
                DEP_UnseenOperator.SetError(PV_PdfToBase64, "Please select valid pdf file for encode!");
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Please select valid pdf file to encode! {ex.Message}", "SimpleOperator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
    private void ACE_DecodePdf_Click(object sender, EventArgs e)
    {
        try
        {
            SetTabPage(XTP_Base64ToPdf);

            if (!string.IsNullOrEmpty(ME_Base64ToPdf.Text))
            {
                PV_Base64ToPdf.LoadDocument(new MemoryStream(Convert.FromBase64String(ME_Base64ToPdf.Text)));
            }
            else
                DEP_UnseenOperator.SetError(ME_Base64ToPdf, "Please select valid PDF to convert");
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Please select valid base64 to decode pdf! {ex.Message}", "SimpleOperator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

    }


    private void ACE_DecodeJwt_Click(object sender, EventArgs e)
    {
        try
        {
            SetTabPage(XTP_JWT);

            if (!string.IsNullOrWhiteSpace(ME_JWT_Input.Text))
            {
                JsonWebTokenHandler handler = new();
                JsonWebToken token = handler.ReadJsonWebToken(ME_JWT_Input.Text);

                string headerJson = Encoding.UTF8.GetString(Convert.FromBase64String(AddPaddingIfNeeded(token.EncodedHeader)));
                string payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(AddPaddingIfNeeded(token.EncodedPayload)));

                JsonDocument parsedHeaderJsonDocument = JsonDocument.Parse(headerJson);
                JsonDocument parsedPayloadJsonDocument = JsonDocument.Parse(payloadJson);

                JsonSerializerOptions serializeSettings = new()
                {
                    WriteIndented = true
                };

                ME_JWT_Header.EditValue = JsonSerializer.Serialize(parsedHeaderJsonDocument, serializeSettings);
                ME_JWT_Payload.EditValue = JsonSerializer.Serialize(parsedPayloadJsonDocument, serializeSettings);
                ME_JWT_Signature.EditValue = token.SigningKey;
            }
            else
                DEP_UnseenOperator.SetError(ME_JWT_Input, "Please input valid JWT value for decode!");
        }
        catch (Exception ex)
        {
            XtraMessageBox.Show($"Please input valid json web token to decode! {ex.Message}", "SimpleOperator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
