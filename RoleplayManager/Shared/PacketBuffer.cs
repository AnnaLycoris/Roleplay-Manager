using System;
using System.Collections.Generic;
using System.Text;

namespace Shared {
    class PacketBuffer : IDisposable {

        #region Properties and Variables

        List<byte> bufferList;
        byte[] readBuffer;
        int readPos;
        bool bufferUpdate = false;

        #endregion

        #region Member Functions

        public PacketBuffer() {
            bufferList = new List<byte>();
            readPos = 0;
        }

        public int GetReadPos() {
            return readPos;
        }

        public byte[] ToArray() {
            return bufferList.ToArray();
        }

        public int Count() {
            return bufferList.Count;
        }

        public int Length() {
            return Count() - readPos;
        }

        public void Clear() {
            bufferList.Clear();
            readPos = 0;
        }

        #endregion

        #region Write Data

        public void WriteBytes(byte[] input) {
            bufferList.AddRange(input);
            bufferUpdate = true;
        }
        public void WriteByte(byte input) {
            bufferList.Add(input);
            bufferUpdate = true;
        }
        public void WriteInteger(int input) {
            bufferList.AddRange(BitConverter.GetBytes(input));
            bufferUpdate = true;
        }
        public void WriteFloat(float input) {
            bufferList.AddRange(BitConverter.GetBytes(input));
            bufferUpdate = true;
        }
        public void WriteString(string input) {
            bufferList.AddRange(BitConverter.GetBytes(input.Length));
            bufferList.AddRange(Encoding.ASCII.GetBytes(input));
            bufferUpdate = true;
        }

        #endregion

        #region Read Data

        public int ReadInteger(bool peek = true) {
            if(bufferList.Count > readPos) {
                if(bufferUpdate) {
                    readBuffer = bufferList.ToArray();
                    bufferUpdate = false;
                }
                int value = BitConverter.ToInt32(readBuffer, readPos);
                if(peek & bufferList.Count > readPos) {
                    readPos += 4;
                }
                return value;
            } else {
                throw new Exception("Buffer out of bounds");
            }
        }
        public float ReadFloat(bool peek = true) {
            if(bufferList.Count > readPos) {
                if(bufferUpdate) {
                    readBuffer = bufferList.ToArray();
                    bufferUpdate = false;
                }
                float value = BitConverter.ToSingle(readBuffer, readPos);
                if(peek & bufferList.Count > readPos) {
                    readPos += 4;
                }
                return value;
            } else {
                throw new Exception("Buffer out of bounds");
            }
        }
        public byte ReadByte(bool peek = true) {
            if(bufferList.Count > readPos) {
                if(bufferUpdate) {
                    readBuffer = bufferList.ToArray();
                    bufferUpdate = false;
                }
                byte value = readBuffer[readPos];
                if(peek & bufferList.Count > readPos) {
                    readPos += 1;
                }
                return value;
            } else {
                throw new Exception("Buffer out of bounds");
            }
        }
        public byte[] ReadBytes(int length,bool peek = true) {
            if(bufferUpdate) {
                readBuffer = bufferList.ToArray();
                bufferUpdate = false;
            }
            byte[] value = bufferList.GetRange(readPos, length).ToArray();
            if(peek & bufferList.Count > readPos) {
                readPos += length;
            }
            return value;
        }
        public string ReadString(bool peek = true) {
            int length = ReadInteger(true);
            if(bufferUpdate) {
                readBuffer = bufferList.ToArray();
                bufferUpdate = false;
            }
            string value = Encoding.ASCII.GetString(readBuffer, readPos, length);
            if(peek & bufferList.Count > readPos) {
                readPos += length;
            }
            return value;
        }

        #endregion

        #region IDisposable

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing) {
            if(!disposedValue) {
                if(disposing) {
                    bufferList.Clear();
                }
                readPos = 0;
            }
            disposedValue = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
