using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Threading;


public class FirebaseManager : Singleton<FirebaseManager>
{
    #region Fields And Properties

    private FirebaseAuth _auth;
    private FirebaseDatabase _database;

    public bool IsReady => _auth != null && _database != null;

    public event Action OnFirebaseReady;
    public event Action OnFirebaseLoadFailed;

    public FirebaseAuth Auth => _auth ??= FirebaseAuth.DefaultInstance;
    public FirebaseDatabase Database => _database ??= FirebaseDatabase.DefaultInstance;

    #endregion


    #region Public Methods

    public async UniTask Initialize(CancellationToken ct)
    {
        var status = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync()
            .AsUniTask()
            .AttachExternalCancellation(ct);

        if (status != Firebase.DependencyStatus.Available)
        {
            throw new Exception($"Firebase 연결 실패: {status}");
        }

        _auth = FirebaseAuth.DefaultInstance;
        _database = FirebaseDatabase.DefaultInstance;
        _database.GoOnline();
    }

    public void GoOfflineRTDB()
    {
        if (_database != null) _database.GoOffline();
    }

    #endregion
}
