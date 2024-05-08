using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Scanner_SDK_Sample_Application
{
    public static class BaseMethods
    {
        /// <summary>
        /// Process the Image object data to Image 
        /// </summary>
        /// <param name="ImageData">object that contains raw image data</param>
        /// <returns>Formatted Image</returns>
        public static Image ProcessImageData(object ImageData)
        {
            try
            {
                Array arr = (Array)ImageData;
                long len = arr.LongLength;
                byte[] byImage = new byte[len];
                arr.CopyTo(byImage, 0);

                MemoryStream ms = new MemoryStream();
                ms.Write(byImage, 0, byImage.Length);

                Image img = Image.FromStream(ms);
                return img;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Get a readable string form the hex string
        /// </summary>
        /// <param name="scanDataLabel">Hex formated string</param>
        /// <returns>Readable string</returns>
        public static string GetReadableScanDataLabel(string scanDataLabel)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] numbers = scanDataLabel.Split(' ');

            foreach (string number in numbers)
            {
                if (String.IsNullOrEmpty(number))
                {
                    break;
                }
                int character = Convert.ToInt32(number, 16);
                stringBuilder.Append(((char)character).ToString());
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// generate the SwitchXml format
        /// </summary>
        /// <param name="inXML">GetOnlyScannerXml()</param>
        /// <param name="strHostMode">Hostmode</param>
        /// <param name="strSilentSwitch">SilentSwitch</param>
        /// <param name="strPermChange">Permission change</param>
        /// <param name="scanerIdRequired">True/False</param>
        /// <returns>XML format</returns>

        public static string GetSwitchXml(string inXML, string strHostMode, string strSilentSwitch, string strPermChange, bool scanerIdRequired = true)
        {
            return "<inArgs>" +
                      (scanerIdRequired ? inXML : "") +
                      "<cmdArgs>" +
                      "<arg-string>" + strHostMode + "</arg-string>" +
                      "<arg-bool>" + strSilentSwitch + "</arg-bool>" +
                      "<arg-bool>" + strPermChange + "</arg-bool>" +
                      "</cmdArgs>" +
                      "</inArgs>";
        }
    }
}
