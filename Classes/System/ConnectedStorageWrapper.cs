#if UNITY_EDITOR
//#define DEBUGENABLE
#endif

#if UNITY_XBOXONE && !UNITY_EDITOR || DEBUGENABLE
using System.Collections.Generic;
using UnityEngine;
using Storage;
using Users;

public static class ConnectedStorageWrapper {

	public delegate void OnSaveDataRetrievedCallback(User user, string name, byte[] data);
	public delegate void OnSaveDataDidNotExistCallback(User user, string name);
	public delegate void OnConnectedStorageReadyCallback(int userId);

	private static Dictionary<int, ConnectedStorage> _storages = new Dictionary<int, ConnectedStorage>();
	private static KeyValuePair<User, string>? currentlyLoadingMap;
	private static Queue<KeyValuePair<User, string>> loadMapQueue = new Queue<KeyValuePair<User, string>>();

	public static OnSaveDataRetrievedCallback OnSaveDataRetrieved;
	public static OnSaveDataDidNotExistCallback OnSaveDataDidNotExist;
	public static OnConnectedStorageReadyCallback OnConnectedStorageReady;

	public static void Create(User user) {
		if (!StorageManager.AmFullyInitialized()) {
			Debug.LogError("Attempted to create Connected Storage for user when storage manager not yet initialized.");
		}
		ConnectedStorage.CreateAsync(user.Id, Application.productName, OnConnectedStorageCreated);
	}

	private static void OnConnectedStorageCreated(ConnectedStorage storage, CreateConnectedStorageOp op) {
		if (op.Success) {
			// Keep this around so we can actually run operations.
			_storages[storage.UserId] = storage;

			storage.OnUserSignedOut += OnUserSignedOut;

			if (OnConnectedStorageReady != null) {
				OnConnectedStorageReady(storage.UserId);
			}
		} else {
			
		}

	}

	public static void SaveData(User user, string name, byte[] data) {
		DataMap toSave = DataMap.Create();
		toSave.AddOrReplaceBuffer(name, data);
		_storages[user.Id].SubmitUpdatesAsync(toSave, null, OnSubmitDone, name);
	}

	public static void LoadData(User user, string name) {
		loadMapQueue.Enqueue(new KeyValuePair<User, string>(user, name));
		LoadNextDataInQueue();
	}

	public static void LoadNextDataInQueue() {
		Debug.Log("LoadNextDataInQueue when there are " + loadMapQueue.Count + " datas left to get in queue, and currentlyLoadingMap is null: " + (currentlyLoadingMap == null));
		if (!currentlyLoadingMap.HasValue && loadMapQueue.Count > 0) {
			KeyValuePair<User, string> pair = loadMapQueue.Dequeue();
			string name = pair.Value;
			Debug.Log("Loading data " + name);
			currentlyLoadingMap = pair;
			_storages[pair.Key.Id].GetAsync(new string[] { name }, OnGetData);
		}
	}

	private static void OnSubmitDone(ContainerContext storage, SubmitDataMapUpdatesAsyncOp op) {
		if (op.Success) { }
	}

	private static void OnUserSignedOut(uint userId, ConnectedStorage storage) {
		storage.OnUserSignedOut -= OnUserSignedOut;
		_storages.Remove(storage.UserId);
	}

	private static void OnGetData(ContainerContext storage, GetDataMapViewAsyncOp op, DataMapView view) {
		Debug.Log("OnGetData for " + currentlyLoadingMap.Value.Value + " so now there are " + loadMapQueue.Count + " left to get.");
		try {
			if (op.Success) {
				if (OnSaveDataRetrieved != null) {
					OnSaveDataRetrieved(currentlyLoadingMap.Value.Key, currentlyLoadingMap.Value.Value, view.GetBuffer(currentlyLoadingMap.Value.Value));
				}
			} else { // Does not exist (like on first start)
				if (OnSaveDataDidNotExist != null) {
					OnSaveDataDidNotExist(currentlyLoadingMap.Value.Key, currentlyLoadingMap.Value.Value);
				}
			}
		} catch { }
		currentlyLoadingMap = null;
		LoadNextDataInQueue();
	}
}
#endif
