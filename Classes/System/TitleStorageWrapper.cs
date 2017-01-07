using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
#if UNITY_XBOXONE
using DataPlatform;
using System.Text;
using Storage;
#endif


public static class TitleStorageWrapper {

	
#if UNITY_XBOXONE && !UNITY_EDITOR
	public static TitleStorage store = null;

	/// <summary>
	/// 2^18 = 256k of memory.
	/// Should be more than enough for any level data we need to save.
	/// </summary>
	public const int MAX_DATA_SIZE = 2 << 18;

	public static void Load(string filename, Action<bool, byte[], int> callback) {
		byte[] data = new byte[MAX_DATA_SIZE];
		if (store != null) {
			store.DownloadFileAsync(filename, data, (storage, op, size) => {
				callback(op.Success, op.Buffer, (int)op.Size);
			});
		}
	}
		

	

	public static void Save(string filename, string data) { Save(filename, GZip.Compress(data)); }
	public static void Save(string filename, byte[] data) {
		if (store != null) {
			store.UploadFileAsync(filename, data, (storage, op, size) => {

			});
		}
	}

	public static void Delete(string filename) {
		if (store != null) {
			store.DeleteFileAsync(filename, (storage,op)=>{
				
			});
		}
	}
#endif
	
	


}
