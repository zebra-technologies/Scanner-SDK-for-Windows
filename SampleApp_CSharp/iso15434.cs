using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Scanner_SDK_Sample_Application
{
    // Valid DocCap and SigCap types
    public enum CaptureTypes
    {
        DocCap = 0, SigCap_Type2 = 2, SigCap_Type5 = 5,
        SigCap_Type7 = 7, SigCap_Type8 = 8, SigCap_Type9 = 9
    };
    public enum usbBus { HID, IMAGE, BULK };

    public enum SsiImageTypes
    {
        Unknown = 0,
        Jpeg = 1,
        Bmp = 3,
        Tiff = 4,
    }

    public class DecodeData : EventArgs
    {
        private CodeTypes codeType;
        private byte[] rawData;
        private string text = "";

        public DecodeData(DecodeData.CodeTypes codeType, byte[] data)
        {
            this.codeType = codeType;
            this.rawData = data;

            text = System.Text.Encoding.Default.GetString(data);
        }

        public DecodeData.CodeTypes CodeType { get { return codeType; } }
        public byte[] RawData { get { return rawData; } }
        public string Text { get { return text; } }

        public enum CodeTypes
        {
            Unknown = 0,
            Code39 = 1,
            Codabar = 2,
            Code128 = 3,
            Discrete2of5 = 4,
            Iata = 5,
            Interleaved2of5 = 6,
            Code93 = 7,
            UpcA = 8,
            UpcE0 = 9,
            Ean8 = 10,
            Ean13 = 11,
            Code11 = 12,
            Code49 = 13,
            Msi = 14,
            Ean128 = 15,
            UpcE1 = 16,
            Pdf417 = 17,
            Code16K = 18,
            Code39FullAscii = 19,
            UpcD = 20,
            Code39Trioptic = 21,
            Bookland = 22,
            CouponCode = 23,
            Nw7 = 24,
            Isbt128 = 25,
            Micro_Pdf = 26,
            DataMatrix = 27,
            QrCode = 28,
            MicroPdfCca = 29,
            PostNetUS = 30,
            PlanetCode = 31,
            Code32 = 32,
            Isbt128Con = 33,
            JapanPostal = 34,
            AustralianPostal = 35,
            DutchPostal = 36,
            MaxiCode = 37,
            CanadianPostal = 38,
            UkPostal = 39,
            MacroPdf = 40,
            Aztec = 45,
            Rss14 = 48,
            RssLimited = 49,
            RssExpanded = 50,
            Scanlet = 55,
            UpcAPlus2 = 72,
            UpcE0Plus2 = 73,
            Ean8Plus2 = 74,
            Ean13Plus2 = 75,
            UpcE1Plus2 = 80,
            CcaEAN128 = 81,
            CcaEAN13 = 82,
            CcaEAN8 = 83,
            CcaRssExpanded = 84,
            CcaRssLimited = 85,
            CcaRss14 = 86,
            CcaUpcA = 87,
            CcaUpcE = 88,
            CccEAN128 = 89,
            Tlc39 = 90,
            CcbEan128 = 97,
            CcbEan13 = 98,
            CcbEan8 = 99,
            CcbRssExpanded = 100,
            CcbRssLimited = 101,
            CcbRss14 = 102,
            CcbUpcA = 103,
            CcbUpcE = 104,
            SignatureCapture = 105,
            Matrix2of5 = 113,
            Chinese2of5 = 114,
            UpcAPlus5 = 136,
            UpcE0Plus5 = 137,
            Ean8Plus5 = 138,
            Ean13Plus5 = 139,
            UpcE1Plus5 = 144,
            MacroMicroPDF = 154,
            MailMark = 195,
            DotCode = 196,
            UDICode = 204,
            NoDecode = 255,
        }
    }

    /// <summary>
    /// Class used to store know parameter definitions
    /// </summary>
    public class ParamDefs : IComparable
    {
        public enum paramClasses { PCT_CODETYPE, PCT_CODELEN, PCT_CODEOPT, PCT_IMAGING, PCT_MISC, PCT_DCSYMBOL };
        public enum paramTypes { PVT_BIT, PVT_BYTE, PVT_WORD, PVT_STRING };

        // Simple getters and setters
        public string Name { get { return this.m_name; } }
        public int Id { get { return this.m_id; } }
        public paramClasses Class { get { return this.m_class; } }
        public paramTypes Type { get { return this.m_type; } }
        public string Descr { get { return this.m_description; } }
        public string Enums { get { return this.m_enums; } }
        public int MinValue { get { return this.m_minValue; } }
        public int MaxValue { get { return this.m_maxValue; } }

        public ParamDefs(string[] items)
        {
            // Line format: ParamName, SsiId, ParamClass, ParamType, Min, Max
            if (items == null) return;
            if (items[0].StartsWith("PARAM_", StringComparison.OrdinalIgnoreCase))
                items[0] = items[0].Substring(6);
            this.m_name = items[0].Trim();
            this.m_id = Int16.Parse(items[1]);
            this.m_class = (paramClasses)Enum.Parse(typeof(paramClasses), items[2]);
            this.m_type = (paramTypes)Enum.Parse(typeof(paramTypes), items[3]);
            this.m_minValue = Int32.Parse(items[4]);
            this.m_maxValue = Int32.Parse(items[5]);
            this.m_description = string.Empty;
            this.m_enums = string.Empty;
            if (items.Length > 6)
                this.m_description = items[6].Trim();
            if (items.Length > 7)
                this.m_enums = items[7].Trim();
        }

        public ParamDefs(int id)
        {
            this.m_name = id.ToString();
            this.m_id = id;
            this.m_class = paramClasses.PCT_CODETYPE;
            this.m_type = paramTypes.PVT_WORD;
            this.m_minValue = 0;
            this.m_maxValue = UInt16.MaxValue;
            this.m_description = string.Empty;
            this.m_enums = string.Empty;
        }
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            ParamDefs otherParamDefs = obj as ParamDefs;
            return string.Compare(this.Name, otherParamDefs.Name);
        }

        public string ToString(int val)
        {
            string s;
            if ((this.m_minValue >= 0) || (this.m_type == ParamDefs.paramTypes.PVT_BIT))
                s = Convert.ToString(val); // unsigned numbers and booleans are easy
            else
            {
                if (this.m_type == ParamDefs.paramTypes.PVT_BYTE)
                    s = Convert.ToString((SByte)val); // signed byte
                else
                    s = Convert.ToString((Int16)val); // signed word
            }
            return s;
        }

        private string m_name;
        private int m_id;
        private paramClasses m_class;
        private paramTypes m_type;
        private int m_minValue;
        private int m_maxValue;
        private string m_description;
        private string m_enums;
    } // ParamDefs

    internal class DocCapMessage
    {
        // Constants for ISO15434 messages
        private const byte ISO_RS = 0x1E; // ISO15454 Format Trailer Character
        private const byte ISO_GS = 0x1D; // ISO15454 Data Element Seperator
        private const byte ISO_EOT = 0x04; // ISO15454 Message Trailer Character
        private const byte MSG_EASYCAP = 0; // ISO15451 Message DocCap message number

        // Setup for Events 
        // Establish delegates
        public delegate void DocCapImageHandler(object sender, DocCapImageArgs e);
        public delegate void DocCapDecodeHandler(object sender, EventArgs e);

        // Create public events
        public event DocCapImageHandler DocCapImage;
        public event DocCapDecodeHandler DocCapDecode;

        /// <summary>
        /// Try processing the Bulk data data as a DocCap/SigCap message
        /// </summary>
        /// <param name="d">Decoded data from USB Bulk channel</param>
        /// <returns>true if processed as a DocCap message, false otherwise</returns>
        public Boolean ParseMessage(UnknownBulkEventArg ube)
        {
            // All data on the BULK Channel is considered to be DocCap
            // 0x53 is a magic number for old DocCap bulk transfer
            if (ube.Format == 0x53)
                processOldDocCapContent(ube.Data, usbBus.BULK);
            else
                processISO15434DocCap(ube.Data, usbBus.BULK);

            return true; // Currently all bulk channel messages are DocCap
        }// ParseMessage

        /// <summary>
        /// Try processing the data as a DocCap/SigCap message
        /// </summary>
        /// <param name="d">Decoded data from USB HID channel</param>
        /// <returns>true if processed as a DocCap message, false otherwise</returns>
        public Boolean ParseMessage(DecodeData d)
        {
            if (d.CodeType == DecodeData.CodeTypes.SignatureCapture)
            {
                processSigCapContent(d.RawData, true);
                return true;
            }

            // Document Capture messages can be formatted as one of two data packets:
            //  - a multi-barcode (0x98) (Obsolete, decoded here for backward compatibility)
            //  - a ISO15434 (0xB5) (Format generated by scanner)
            if (d.CodeType == (DecodeData.CodeTypes)0x98) // BT_MULTI_BARCODE_SSI_PACKET_TYPE
            {
                processOldDocCapContent(d.RawData, usbBus.HID);
                return true;
            }

            if (d.CodeType == (DecodeData.CodeTypes)0xB5) // BT_ISO15434 (New DocCap format)
            {
                processISO15434DocCap(d.RawData, usbBus.HID);
                return true;
            }

            return false;
        } // ParseMessage

        /// <summary>
        /// try to interpret this data as a Signature capture 
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="theBus"></param>
        private void processSigCapContent(byte[] rawData, bool onHID)
        {
            // Signature capture format:
            //   byte0     - FMT type (BMP,TIFF,JPEG)   <Will check here>
            //   byte1     - signature capture type     <Will check here>
            //   byte2-5   - image length (big endian)
            //   bytes 6+  - Image data

            try
            {
                // Is the type correct?
                if ((CaptureTypes)rawData[1] == CaptureTypes.DocCap)
                    throw new Exception("BarCode symbology says SigCap, but message type says DocCap");

                if (!Enum.IsDefined(typeof(CaptureTypes), (Int32)rawData[1]))
                    throw new Exception("Invalid Type: " + rawData[1]);

                // Does the file type look OK ? Not done in SendCaptureImage, since it may change for other DocCap
                if (!Enum.IsDefined(typeof(SsiImageTypes), (Int32)rawData[0]))
                    throw new Exception("Invalid File Format: " + rawData[0]);

                // Looks OK, send the event
                SendCaptureImage(rawData, 0, onHID);
            }
            catch (Exception e)
            {
                throw new Exception("in a SigCap message: " + e.Message);
            }
        } // processSigCapContent

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="theBus"></param>
        private void processOldDocCapContent(byte[] rawData, usbBus theBus)
        {
            // length of first barcode (plus its symtype leader)
            int offset = (rawData[2] << 8) | rawData[3];

            // starting position of the 2nd barcode content
            int index = 4 + offset;

            bool isValid =
                // must have 2 barcodes
               (rawData[0] == 2)
                // second barcode must be a SigCap type (69H)
               && ((index < (rawData.Length - 3)) && (rawData[index + 3] == 0x69));

            if (!isValid)
                throw new Exception("unknown symbology type/format(legacy message)");

            // Offset is now pointing at the beginning of the first chunk, 
            // Each packet has a 4 header:
            //        byte0   - continue? (0=last chunck)
            //        byte1,2 - length of packet data
            //        byte3   - barcode type

            // The first chunk (only), has a special Sigcap header of 6 bytes:
            //        byte0   - FMT type (BMP,TIFF,JPEG)
            //        byte1   - 0 (DocCap)
            //        byte2-5 - image length (big endian)

            // Get the total length of the image from the Sigcap header
            int totalLength = (rawData[index + 6] << 24) |
                              (rawData[index + 7] << 16) |
                              (rawData[index + 8] << 08) |
                               rawData[index + 9];

            // .. And allocate a memory stream for it, and assign the initial data,
            //    which starts after the 4 and 6 byte headers described above. 
            int chunkSt = index; // Chunk start
            int chunkLen = ((rawData[chunkSt + 1] << 8) | rawData[chunkSt + 2]) - 1;

            try
            {
                rawData[index + 5] = 0; // Make sure it says DocCap
                MemoryStream ms = new MemoryStream();
                ms.SetLength(totalLength);
                ms.Write(rawData, chunkSt + 4, chunkLen);

                // Point to the next chunk, which may not exist
                chunkSt += (chunkLen + 4);

                // Process multiple chunks, if present
                while (chunkSt < rawData.Length)
                {
                    // extract the next chunk's length and advance our offset past the header
                    chunkLen = ((rawData[chunkSt + 1] << 8) | rawData[chunkSt + 2]) - 1;

                    // append this next chunk to our memory buffer
                    if ((chunkSt + chunkLen) < rawData.Length)
                        ms.Write(rawData, chunkSt + 4, chunkLen);

                    // advance to (potentially) next chunk
                    chunkSt += (chunkLen + 4);
                }

                // Display the decoded image
                // Looks OK, send the event to display the message
                SendCaptureImage(ms.ToArray(), 0, theBus == usbBus.HID);

                // Send the decoded data
                byte[] decRaw = new byte[offset - 1];
                Array.Copy(rawData, 5, decRaw, 0, offset - 1);
                DecodeData d = new DecodeData((DecodeData.CodeTypes)(rawData[0]), decRaw);
                DocCapDecode(this, d); // fire the event
            }
            catch
            {
                throw new Exception("Image conversion error (legacy message)");
            }
        } // processOldDocCapContent

        private void processISO15434DocCap(byte[] rawData, usbBus theBus)
        {
            // Validate the basic message structure first
            try
            {
                // Header: is PacketLength correct?
                //   Packet length does not include itself in the count
                int packetLength = (rawData[0] << 24) |
                                   (rawData[1] << 16) |
                                   (rawData[2] << 08) |
                                    rawData[3];
                if (packetLength + 4 != rawData.Length)
                    throw new Exception("bytes received doesn't match packet length (" + (packetLength + 4) + " vs " + rawData.GetLength(0) + ")");

                // Header: is Message Type correct ?
                if (rawData[4] != MSG_EASYCAP)
                    throw new Exception("invalid MSG type. Got " + rawData[4] + " expected " + MSG_EASYCAP);

                // ISO15434 Envelope: is message header correct?
                if ((rawData[5] != '[') || (rawData[6] != ')') || (rawData[7] != '>') || (rawData[8] != ISO_RS))
                    throw new Exception("invalid MSG header");

                // ISO15434 Envelope: is message header correct?
                if (rawData[rawData.Length - 1] != ISO_EOT)
                    throw new Exception("invalid MSG trailer. Got " + rawData[rawData.Length - 1] + " expected " + ISO_EOT);
            }
            catch (Exception e)
            {
                throw new Exception("Bad ISO15434 envelope: " + e.Message);
            }

            // At this point the basic header and ISO15434 has been validated. 
            // Now we can extract the format envelope(s)and call the appropriate function for processing. 
            // We currently expect and support only two types: Bar Code and Image. We can get none or multiples of either.
            try
            {
                ISO15434formatEnvelope anEnvelope = new ISO15434formatEnvelope(rawData, 9);
                while (anEnvelope.getNext())
                {
                    if (anEnvelope.getFileType() == "BarCode")
                    {
                        // We have extracted bar code data - send the event so it can be displayed
                        byte[] decRaw = new byte[anEnvelope.getDataLength() - 1];
                        Array.Copy(rawData, anEnvelope.getDataIndex() + 1, decRaw, 0, anEnvelope.getDataLength() - 1);
                        DecodeData d = new DecodeData((DecodeData.CodeTypes)(rawData[anEnvelope.getDataIndex()]), decRaw);
                        DocCapDecode(this, d); // fire the event
                    }
                    else
                    {
                        // Display the image, which is an specialized Signature capture format:
                        // byte0   = Bitmap typeof (BMP/TIFF/JPEG)
                        // byte1   = Signature capture type (0 = DocCap)
                        // byte2-5 = Size of image data
                        // byte6.. = image data
                        SsiImageTypes thisFormat;
                        int i = anEnvelope.getDataIndex();
                        int bitmapLength = (rawData[i + 2] << 24) |
                                           (rawData[i + 3] << 16) |
                                           (rawData[i + 4] << 08) |
                                            rawData[i + 5];
                        // Verify some of the data. Not required for application, but a nice sanity check
                        thisFormat = (SsiImageTypes)rawData[i];
                        if (thisFormat.ToString().ToLower() != anEnvelope.getFileType().ToLower())
                            throw new Exception("BITMAP type mismatch between bitmap and ISO msg:" + thisFormat + " vs " + anEnvelope.getFileType());

                        if (rawData[i + 1] != 0)
                            throw new Exception("expected sigcap type of 0. Got " + rawData[i + 1]);
                        if (bitmapLength + 6 != anEnvelope.getDataLength()) // the extra six bytes are those listed above.
                            throw new Exception("length mismatch between bitmap and ISO msg:" + (bitmapLength + 6) + " vs " + anEnvelope.getDataLength());

                        // Looks OK, send the event to display the message
                        SendCaptureImage(rawData, anEnvelope.getDataIndex(), theBus == usbBus.HID);
                    }
                } // while (anEnvelope.getNext())
            }
            catch (Exception e)
            {
                throw new Exception("Bad ISO15434 message: " + e.Message);
            }

        } // processISO15434Message

        /// <summary>
        /// Parse data as an ISO15434 message format envelope
        /// </summary>
        private class ISO15434formatEnvelope
        {

            public ISO15434formatEnvelope(byte[] rawData, int i)
            {
                this.data = rawData;
                this.index = i;
            }
            public string getFileType() { return this.fileType; }
            public int getDataLength() { return this.dataLength; }
            public int getDataIndex() { return this.dataIndex; }

            /// <summary>
            /// Get the next ISO15434 envelope, if it exists
            /// </summary>
            /// <returns>true if another envelope exists, false otherwise</returns>
            public Boolean getNext()
            {
                // Get the next format envelope of interest. Right now we only want '09' (binary data) and 
                // all others will be skipped if possible [probably want to print this event during testing].
                // Return true if an 09 envelope is found, false otherwise 
                Boolean gotAn09 = false;
                do
                {
                    if (data[index] == ISO_EOT) return false; // now more envelopes here
                    // Convert the format indicator to a number and validate
                    try
                    {
                        int indicator = int.Parse(Encoding.Default.GetString(data, index, 2));
                        switch (indicator)
                        {
                            case 1:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                // These formats terminate with an ISO_RS, so we can skip them
                                index = Array.IndexOf(data, ISO_RS, index);
                                if (index < 0)
                                    throw new Exception("no Rs found while skipping unsupported envelope " + indicator);
                                index++; // Skip past the ISO_RS
                                break;
                            case 9:
                                // This is what we want! Let's get out of this loop
                                gotAn09 = true;
                                break;
                            case 2:
                            default:
                                // These formats can not be skipped, either they are Reserved for future use 
                                // (0, 10-11, 12-99) or format 2 (Transportation) which is the only format in this message
                                throw new Exception("unexpected envelope: " + indicator);
                        } // switch
                    }
                    catch (Exception e)
                    {
                        throw new Exception("error parsing format indicator: " + e.Message);
                    }
                } while (!gotAn09);

                // Hey, we finally got a Binary data format envelope, now we can process it
                int curIndex = index + 2; // Point past the two characters of the format indicator
                int searchIndex;
                try
                {
                    // The next character better be a ISO_GS
                    if (data[curIndex] != ISO_GS)
                        throw new Exception("expecting Gs after indicator, got " + data[curIndex]);

                    // Extract the file type name (ttt...t)
                    searchIndex = Array.IndexOf(data, ISO_GS, ++curIndex);
                    fileType = Encoding.Default.GetString(data, curIndex, searchIndex - curIndex);

                    // Dump the compression technique name (ccc...c)
                    curIndex = searchIndex + 1;
                    searchIndex = Array.IndexOf(data, ISO_GS, curIndex);

                    // Extract and convert the number of bytes (nnn...n)
                    curIndex = searchIndex + 1;
                    searchIndex = Array.IndexOf(data, ISO_GS, curIndex);
                    string cnt = Encoding.Default.GetString(data, curIndex, searchIndex - curIndex);

                    // Update the variables for the data
                    dataLength = int.Parse(cnt);
                    dataIndex = searchIndex + 1;

                    // Final check - the character after the data should be ISO_RS
                    if (data[dataIndex + dataLength] != ISO_RS)
                        throw new Exception("missing Rs after data, got " + data[dataIndex + dataLength]);
                    index = dataIndex + dataLength + 1; // setup for next time

                }
                catch (Exception e)
                {
                    throw new Exception("error parsing 09 envelope: " + e.Message);
                }

                return true; // found a valid entry if we got to here.
            }

            // Internal (private) variables
            private byte[] data; // input data from USB
            private int index; // current index into data[] of latest envelope
            private string fileType; // File type name extracted from 09 envelope.
            private int dataLength;
            private int dataIndex;
        } // ISO15434formatEnvelope

        /// <summary>
        /// Format and send the DocCapImage event
        /// </summary>
        private void SendCaptureImage(byte[] rawData, int offset, bool onHID)
        {
            // Signature capture format:
            //   byte0     - FMT type (BMP,TIFF,JPEG)  <Already checked>
            //   byte1     - signature capture type    <Already checked>
            //   byte2-5   - image length (big endian) <Will check here>
            //   bytes 6+  - Image data

            // Make sure that the length looks OK
            int length = (rawData[offset + 2] << 24) |
                         (rawData[offset + 3] << 16) |
                         (rawData[offset + 4] << 08) |
                          rawData[offset + 5];

            if (length + offset + 6 > rawData.Length)
                throw new Exception("Messsage too short based on image size field");

            // Format and send the event 
            byte[] imgData = new byte[length];
            Array.Copy(rawData, offset + 6, imgData, 0, length);
            DocCapImageArgs dce = new DocCapImageArgs((CaptureTypes)rawData[offset + 1], (SsiImageTypes)rawData[offset + 0], imgData, onHID);
            if (DocCapImage != null) DocCapImage(this, dce); // fire the event
        } // SendCaptureImage

    }

    /// <summary>
    /// Events arguments for Document Capture image events
    /// </summary>
    public class DocCapImageArgs : System.EventArgs
    {
        public DocCapImageArgs(CaptureTypes msgType, SsiImageTypes imgType, byte[] imageData, bool onHID)
        {
            this.m_msgType = msgType;
            this.m_imgType = imgType;
            this.m_imageData = imageData;
            this.m_onHID = onHID;
        }

        // Simple getters and setters
        public CaptureTypes MsgType { get { return this.m_msgType; } }
        public SsiImageTypes ImgType { get { return this.m_imgType; } }
        public byte[] ImgData { get { return this.m_imageData; } }
        public bool OnHID { get { return this.m_onHID; } }

        // Internal (private) variables
        private CaptureTypes m_msgType;
        private SsiImageTypes m_imgType;
        private byte[] m_imageData;
        private bool m_onHID;
    }

    public class UnknownBulkEventArg : EventArgs
    {
        public UnknownBulkEventArg(byte[] rawData)
        {
            this.data = rawData;
            this.format = 0x00;
        }
        private byte[] data=default;
        private byte flags = default;
        private byte format = default;
        private int height = default;
        private int size = default;
        private int widh = default;

        public byte[] Data
        {
            get { return data; }
        }
        public byte Flags
        { get { return flags; } }
        public byte Format { get { return format; } }
        public int Height { get { return height; } }
        public int Size { get { return size; } }
        public int Width { get { return widh; } }
    }
}
