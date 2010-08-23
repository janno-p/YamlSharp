using System.IO;
using System.Text;
using System;

namespace YamlSharp
{
	enum StreamPosition
	{
		DocumentPrefix,
		DirectivesEnd,
		DocumentEnd,
		DocumentSuffix,
		Directives,
		DocumentContent,
	}
	
	public class Scanner
	{
		private const int BUFFER_SIZE = 1024;
		
		private readonly byte[] buffer = new byte[BUFFER_SIZE];
		private readonly Stream stream;
		
		private Encoding encoding;
		private int bufferCursor;
		private StreamPosition streamPosition = StreamPosition.DocumentPrefix;
		
		public Scanner(Stream stream)
		{
			this.stream = stream;
		}
		
		public object ReadNextToken()
		{
			UpdateBuffer();
			
			switch (streamPosition)
			{
			case StreamPosition.DocumentPrefix:
				ReadEncoding();
				ReadCommentLines();
				break;
			}
			
			return null;
		}
		
		#region c-byte-order-mark
		
		private void ReadEncoding()
		{
			Encoding newEncoding = ReadByteOrderMark();
			if (encoding != null && newEncoding != encoding)
				throw new Exception("Must be same encoding in single thread.");
			encoding = newEncoding;
		}
		
		private Encoding ReadByteOrderMark()
		{
			UpdateBuffer();
			
            if (buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0xFE && buffer[3] == 0xFF)
			{
				bufferCursor += 4;
                return new UTF32Encoding(true, true, true);
			}
            if (buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0x00)
                return new UTF32Encoding(true, false, true);
            if (buffer[0] == 0xFF && buffer[1] == 0xFE && buffer[2] == 0x00 && buffer[3] == 0x00)
			{
				bufferCursor += 4;
                return new UTF32Encoding(false, true, true);
			}
            if (buffer[1] == 0x00 && buffer[2] == 0x00 && buffer[3] == 0x00)
                return new UTF32Encoding(false, false, true);
            if (buffer[0] == 0xFE && buffer[1] == 0xFF)
			{
				bufferCursor += 2;
                return new UnicodeEncoding(true, true, true);
			}
            if (buffer[0] == 0x00)
                return new UnicodeEncoding(true, false, true);
            if (buffer[0] == 0xFF && buffer[1] == 0xFE)
			{
				bufferCursor += 2;
                return new UnicodeEncoding(false, true, true);
			}
            if (buffer[1] == 0x00)
                return new UnicodeEncoding(false, false, true);
            if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
			{
				bufferCursor += 3;
                return new UTF8Encoding(true, true);
			}
            return new UTF8Encoding(false, true);
		}
		
		#endregion
		
		#region l-comment
		
		public void ReadCommentLines()
		{
			
		}
		
		#endregion
		
		private void UpdateBuffer()
		{
			if (bufferCursor == 0)
				return;
			
			var available = BUFFER_SIZE - bufferCursor;
			
			var temp = new byte[available];
			Array.Copy(buffer, bufferCursor, temp, 0, available);
			Array.Copy(temp, 0, buffer, 0, available);
			
			int c;
			while (available < BUFFER_SIZE && (c = stream.ReadByte()) != -1)
				buffer[available] = (byte)c;
		}
	}
}

