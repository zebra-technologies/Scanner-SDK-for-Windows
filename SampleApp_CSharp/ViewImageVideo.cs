using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using CoreScanner;
using System.Text.RegularExpressions;

namespace Scanner_SDK_Sample_Application
{
    public partial class frmScannerApp
    {
        private void PerformBtnImageClick(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
                pbxImageVideo.Enabled = true;
                pbxImageVideo.Image = null;
                string inXml = GetScannerIDXml();
                int opCode = DEVICE_CAPTURE_IMAGE;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "SET_IMAGE_MODE");
            }
        }


        private void PerformBtnBarcodeClick(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
                string inXml = GetScannerIDXml();
                int opCode = DEVICE_CAPTURE_BARCODE;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "SET_BARCODE_MODE");
            }
        }

        private void PerformOnVideoViewFinderEnable(object sender, EventArgs e)
        {
            if (!IsMotoConnectedWithScanners())
            {
                return;
            }
            m_arViewFindParamsList[0] = VIDEOVIEWFINDER_PARAMNUM;
            if (chkVideoViewFinderEnable.Checked)
            {
                m_arViewFindParamsList[1] = VIDEOVIEWFINDER_ON;
            }
            else
            {
                m_arViewFindParamsList[1] = VIDEOVIEWFINDER_OFF;
            }

            string inXml = "<inArgs>" +
                    GetOnlyScannerIDXml() +
                    "<cmdArgs>" +
                      "<arg-xml>" +
                        "<attrib_list>" +
                          "<attribute>" +
                            "<id>" + m_arViewFindParamsList[0] + "</id>" +
                            "<datatype>B</datatype>" +
                            "<value>" + m_arViewFindParamsList[1] + "</value>" +
                          "</attribute>" +
                        "</attrib_list>" +
                      "</arg-xml>" +
                    "</cmdArgs>" +
                  "</inArgs>";

            int opCode = DEVICE_SET_PARAMETERS;
            string outXml = "";
            int status = STATUS_FALSE;
            ExecCmd(opCode, ref inXml, out outXml, out status);
            DisplayResult(status, "SET_PARAMETERS");
        }


        private void PerformBtnVideoClick(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
                pbxImageVideo.Enabled = true;
                pbxImageVideo.Image = null;
                string inXml = GetScannerIDXml();
                int opCode = DEVICE_CAPTURE_VIDEO;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "SET_VIDEO_MODE");
            }
        }

        private void PerformBtnAbortImageXferClick(object sender, EventArgs e)
        {
            if (IsMotoConnectedWithScanners())
            {
                pbxImageVideo.Image = null;
                pbxImageVideo.Enabled = false;
                string inXml = GetScannerIDXml();
                int opCode = ABORT_IMAGE_XFER;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "ABORT_IMAGE_XFER");
            }
        }

        private void PerformBtnSveImgeClick(object sender, EventArgs e)
        {
            if (saveImgFileDialog.ShowDialog() == DialogResult.OK && imgCapturedImage != null)
            {
                imgCapturedImage.Save(saveImgFileDialog.FileName);
            }
        }

        private void PerformOnJpg(object sender, EventArgs e)
        {
            if (rdoJPG.Checked)
            {
                if (!IsMotoConnectedWithScanners())
                {
                    return;
                }


                string inXml = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                    "<cmdArgs>" +
                                        "<arg-xml>" +
                                            "<attrib_list>" +
                                                "<attribute>" +
                                                    "<id>" +
                                                      IMAGE_FILETYPE_PARAMNUM +
                                                    "</id>" +
                                                    "<datatype>B</datatype>" +
                                                    "<value>" +
                                                      JPEG_FILE_SELECTION +
                                                    "</value>" +
                                                 "</attribute>" +
                                              "</attrib_list>" +
                                           "</arg-xml>" +
                                       "</cmdArgs>" +
                                     "</inArgs>";

                int opCode = DEVICE_SET_PARAMETERS;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "SET_PARAMETERS");
            }
        }

        private void PerformOnTiff(object sender, EventArgs e)
        {
            if (rdoTIFF.Checked)
            {
                if (!IsMotoConnectedWithScanners())
                {
                    return;
                }
                //string inXml = "<inArgs>" +
                //                 GetOnlyScannerIDXml() +
                //                 "<cmdArgs>" +
                //                 "<arg-int>" + IMAGE_FILETYPE_PARAMNUM + "</arg-int>" +
                //                 "<arg-int>" + TIFF_FILE_SELECTION + "</arg-int>" +
                //                 "</cmdArgs>" +
                //                 "</inArgs>";

                string inXml = "<inArgs>" +
                                    GetOnlyScannerIDXml() +
                                    "<cmdArgs>" +
                                        "<arg-xml>" +
                                            "<attrib_list>" +
                                                "<attribute>" +
                                                    "<id>" +
                                                      IMAGE_FILETYPE_PARAMNUM +
                                                    "</id>" +
                                                    "<datatype>B</datatype>" +
                                                    "<value>" +
                                                      TIFF_FILE_SELECTION +
                                                    "</value>" +
                                                 "</attribute>" +
                                              "</attrib_list>" +
                                           "</arg-xml>" +
                                       "</cmdArgs>" +
                                     "</inArgs>";

                int opCode = DEVICE_SET_PARAMETERS;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "SET_PARAMETERS");
            }
        }

        private void PerformOnBmp(object sender, EventArgs e)
        {
            if (rdoBMP.Checked)
            {
                if (!IsMotoConnectedWithScanners())
                {
                    return;
                }

                string inXml = "<inArgs>" +
                                   GetOnlyScannerIDXml() +
                                   "<cmdArgs>" +
                                       "<arg-xml>" +
                                           "<attrib_list>" +
                                               "<attribute>" +
                                                   "<id>" +
                                                     IMAGE_FILETYPE_PARAMNUM +
                                                   "</id>" +
                                                   "<datatype>B</datatype>" +
                                                   "<value>" +
                                                     BMP_FILE_SELECTION +
                                                   "</value>" +
                                                "</attribute>" +
                                             "</attrib_list>" +
                                          "</arg-xml>" +
                                      "</cmdArgs>" +
                                    "</inArgs>";

                int opCode = DEVICE_SET_PARAMETERS;
                string outXml = "";
                int status = STATUS_FALSE;
                ExecCmd(opCode, ref inXml, out outXml, out status);
                DisplayResult(status, "SET_PARAMETERS");
            }
        }



        /// <summary>
        /// Set the captured image format
        /// </summary>
        /// <param name="img">Captured image</param>
        private void SetImageType(Image img)
        {
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
            {
                saveImgFileDialog.FileName = "*.jpg";
                saveImgFileDialog.Filter = "JPeg Image|*.jpg";
            }
            else if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
            {
                saveImgFileDialog.FileName = "*.bmp";
                saveImgFileDialog.Filter = "Bitmap Image|*.bmp";
            }
            else if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
            {
                saveImgFileDialog.FileName = "*.tif";
                saveImgFileDialog.Filter = "Tiff Image|*.tif";
            }
        }

        /// <summary>
        /// ImageEvent is received
        /// </summary>
        /// <param name="eventType">Type of event</param>
        /// <param name="size">Size of image data buffer</param>
        /// <param name="imageFormat">Format of image</param>
        /// <param name="sfimageData">Image data buffer</param>
        void OnImageEvent(short eventType, int size, short imageFormat, ref object sfimageData, ref string pScannerData)
        {
            try
            {
                if (IMAGE_COMPLETE == eventType)
                {
                    Array arr = (Array)sfimageData;
                    long len = arr.LongLength;
                    byte[] byImage = new byte[len];
                    arr.CopyTo(byImage, 0);

                    MemoryStream ms = new MemoryStream();
                    ms.Write(byImage, 0, byImage.Length);

                    Image img = Image.FromStream(ms);
                    pbxImageVideo.Image = img;
                    UpdateResults("Image Event fired");
                    UpdateOutXml(pScannerData);
                    imgCapturedImage = img;

                    SetImageType(img);
                    
                }
                else
                {
                    int iMax = 0, iProgress = 0;
                    string strStatus = String.Empty, strScannerID = String.Empty;

                    m_xml.ReadXmlString_FW(pScannerData, out iMax, out iProgress, out strStatus, out strScannerID);
                    UpdateResults("Image Event fired - Progress " + iMax.ToString() + ":" + iProgress.ToString());
                    UpdateOutXml(pScannerData);
                }
            }
            catch (Exception)
            {
            }
            if (imgCapturedImage != null)
            {
                btnSveImge.Enabled = true;
            }
            else
            {
                btnSveImge.Enabled = false;
            }
        }

        /// <summary>
        /// VideoEvent is received
        /// </summary>
        /// <param name="eventType">Type of event</param>
        /// <param name="size">Size of video buffer</param>
        /// <param name="sfvideoData">Video data buffer</param>
        void OnVideoEvent(short eventType, int size, ref object sfvideoData, ref string pScannerData)
        {
            try
            {
                Array arr = (Array)sfvideoData;
                long len = arr.LongLength;
                byte[] byImage = new byte[size];
                arr.CopyTo(byImage, 0);

                MemoryStream ms = new MemoryStream();
                ms.Write(byImage, 0, byImage.Length);

                Image img = Image.FromStream(ms);
                pbxImageVideo.Image = img;

                UpdateOutXml(pScannerData);
            }
            catch (Exception)
            {
            }
        }
    }
}
