using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Unity.IO.Compression;

public static class GZip {
	
	/// <summary> Compresses a string using GZip. Uses UTF8 encoding to convert the string to a byte[] </summary>
	/// <param name="data">string to compress. </param>
	/// <returns>GZip Compressed version of data</returns>
	public static byte[] Compress(string data) { return Compress(Encoding.UTF8.GetBytes(data)); }
	/// <summary> Compresses a byte[] using GZip </summary>
	/// <param name="data">byte[] to compress</param>
	/// <returns>GZip Compressed version of data</returns>
	public static byte[] Compress(byte[] data) {
		byte[] compressed;
		using (var outStream = new MemoryStream(data.Length / 4)) {
			using (var gz = new GZipStream(outStream, CompressionMode.Compress)) {
				gz.Write(data, 0, data.Length);
			}
			compressed = outStream.ToArray();
		}
		return compressed;
	}

	/// <summary> Decompresses a GZip byte[], and attempts to convert the output into a UTF8 string. </summary>
	/// <param name="data">GZip byte[] to decompress</param>
	/// <returns>A string containing the UTF8 representation of the decompressed data </returns>
	public static string DecompressString(byte[] data) { return Encoding.UTF8.GetString(Decompress(data)); }

	/// <summary> Decompresses a GZip byte[] </summary>
	/// <param name="data">GZip byte[] to decompress</param>
	/// <returns>Decompressed version of the input data</returns>
	public static byte[] Decompress(byte[] data) {
		using (var inStream = new MemoryStream(data)) {
			using (var gz = new GZipStream(inStream, CompressionMode.Decompress)) {
				const int size = 4096;
				byte[] buffer = new byte[size];
				using (MemoryStream mem = new MemoryStream(size)) {
					int count = 0;
					do {
						count = gz.Read(buffer, 0, size);
						if (count > 0) { 
							mem.Write(buffer, 0, count);
						}
					} while (count > 0);
					return mem.ToArray();
				}
			}
		}
		
	}

	
	
}
