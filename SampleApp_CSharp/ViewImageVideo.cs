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
        const int DEVICE_CAPTURE_IMAGE = 3000;
        const int DEVICE_CAPTURE_BARCODE = 3500;
        const int DEVICE_CAPTURE_VIDEO = 4000;
        const ushort VIDEOVIEWFINDER_PARAMNUM = 0x0144;
        const ushort VIDEOVIEWFINDER_ON = 0x0001; /* Video view finder on */
        const ushort VIDEOVIEWFINDER_OFF = 0x0000; /* Video view finder off */
        const int ABORT_IMAGE_XFER = 3001;
        const ushort IMAGE_FILETYPE_PARAMNUM = 0x0130; /* These values may change with the scanner  */

        private void PerformBtnImageClick(object sender, EventArgs e)
        {
            ExecuteActionCommand(DEVICE_CAPTURE_IMAGE, "SET_IMAGE_MODE");
            pbxImageVideo.Enabled = true;
            pbxImageVideo.Image = null;
        }

        private void PerformOnVideoViewFinderEnable(object sender, EventArgs e)
        {
            if (!IsScannerConnected())
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
            SetParameters(m_arViewFindParamsList[0], m_arViewFindParamsList[1]);
           
        }

        private void SetParameters(ushort id, ushort value)
        {
            string inXml = "<inArgs>" +
                   GetOnlyScannerIDXml() +
                   "<cmdArgs>" +
                     "<arg-xml>" +
                       "<attrib_list>" +
                         "<attribute>" +
                           "<id>" + id + "</id>" +
                           "<datatype>B</datatype>" +
                           "<value>" + value + "</value>" +
                         "</attribute>" +
                       "</attrib_list>" +
                     "</arg-xml>" +
                   "</cmdArgs>" +
                 "</inArgs>";
            ExecuteActionCommand(DEVICE_SET_PARAMETERS, "SET_PARAMETERS", inXml);
        }


        private void PerformBtnVideoClick(object sender, EventArgs e)
        {
            ExecuteActionCommand(DEVICE_CAPTURE_VIDEO, "SET_VIDEO_MODE");
            pbxImageVideo.Enabled = true;
            pbxImageVideo.Image = null;
        }

        private void PerformBtnAbortImageXferClick(object sender, EventArgs e)
        {
            ExecuteActionCommand(ABORT_IMAGE_XFER, "ABORT_IMAGE_XFER");
            pbxImageVideo.Image = null;
            pbxImageVideo.Enabled = false;
        }

        private void PerformBtnSveImgeClick(object sender, EventArgs e)
        {
            if (saveImgFileDialog.ShowDialog() == DialogResult.OK && imgCapturedImage != null)
            {
                imgCapturedImage.Save(saveImgFileDialog.FileName);
            }
        }

        /// <summary>
        /// Set parameters based on file type
        /// </summary>
        /// <param name="FileSelection">File type</param>
        private void PerformOnImageType(ushort FileSelection)
        {
            if (!IsScannerConnected())
            {
                return;
            }
            SetParameters(IMAGE_FILETYPE_PARAMNUM, FileSelection);
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
                    Image img = BaseMethods.ProcessImageData(sfimageData);
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
                pbxImageVideo.Image = BaseMethods.ProcessImageData(sfvideoData);
                UpdateOutXml(pScannerData);
            }
            catch (Exception)
            {
            }
        }
    }
}
