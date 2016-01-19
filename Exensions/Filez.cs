using UnityEngine;
using Unity.IO.Compression;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public static class Filez {

	/// <summary> Writes GZIP Compressed UTF8 text to a file. </summary>
	/// <param name="filename">File path to write to </param>
	/// <param name="content">Text string to write</param>
	public static void WriteGzipText(string filename, string content) {
		byte[] bytes = Encoding.UTF8.GetBytes(content);
		WriteGzipBytes(filename, bytes);
	}

	/// <summary> Writes GZIP Compressed btyes to a file. </summary>
	/// <param name="filename">File path to write to </param>
	/// <param name="content">byte[] to write</param>
	public static void WriteGzipBytes(string filename, byte[] content) {
		string temp = Path.GetTempFileName();
		File.WriteAllBytes(temp, content);

		byte[] b;
		using (FileStream f = new FileStream(temp, FileMode.Open)) {
			b = new byte[f.Length];
			f.Read(b, 0, (int)f.Length);
		}


		using (FileStream f2 = new FileStream(filename, FileMode.Create))
		using (GZipStream gz = new GZipStream(f2, CompressionMode.Compress, false)) {
			gz.Write(b, 0, b.Length);
		}

	}

	/// <summary> Read a GZIP compressed UTF8 string from a given file </summary>
	/// <param name="filename">File path to read from </param>
	/// <returns>UTF8 Encoded string in file.</returns>
	public static string ReadGzipText(string filename) {
		return Encoding.UTF8.GetString(ReadGzipBytes(filename));
	}

	/// <summary> Read a GZIP compressed byte[] from a given file </summary>
	/// <param name="filename">File path to read from </param>
	/// <returns>byte[] of all data in file. </returns>
	public static byte[] ReadGzipBytes(string filename) {
		byte[] loaded = File.ReadAllBytes(filename);

		using (GZipStream stream = new GZipStream(new MemoryStream(loaded), CompressionMode.Decompress)) {
			const int size = 4096;
			byte[] buffer = new byte[size];
			using (MemoryStream memory = new MemoryStream()) {
				int count = 0;
				do {
					count = stream.Read(buffer, 0, size);
					if (count > 0) {
						memory.Write(buffer, 0, count);
					}
				} while (count > 0);
				return memory.ToArray();
			}
		}

	}
	
}
