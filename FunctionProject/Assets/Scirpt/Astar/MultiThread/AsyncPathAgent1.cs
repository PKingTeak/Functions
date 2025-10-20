using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncPathAgent1 : MonoBehaviour
{
    public GridGraph grid;
    public float moveSpeed = 2f;
    public bool allowDiagonal = false;


    CancellationTokenSource cts; //이게 뭔가?

    public void GoToWorldAsync(Vector3 targetWorld)
    {
        Vector2Int start = grid.WorldToCell(transform.position);
        Vector2Int goal = grid.WorldToCell(targetWorld);
        if (!grid.InBounds(start) || !grid.InBounds(goal))
        {
            return; //범위 밖
        }

        if (!grid.Get(start).walkable || !grid.Get(goal).walkable)
        {
            return; //도달 불가
        }

        var snap = grid.MakeSnapShot();
        cts?.Cancel(); //취소
        cts = new CancellationTokenSource(); //새로운 토큰 소스
        var token = cts.Token;


        Task.Run(() =>
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var path = ThreadSafeAStar.FindPath(
                snap.map, snap.w, snap.h,
                new ThreadSafeAStar.I2(start.x, start.y),
                new ThreadSafeAStar.I2(goal.x, goal.y),
                allowDiagonal
            );
            sw.Stop();

            if (token.IsCancellationRequested) return;

            // 결과를 Unity 메인 스레드에서 사용
            UnityMainThread(path, sw.ElapsedMilliseconds);

        }, token);






    }

    void UnityMainThread(List<ThreadSafeAStar.I2> path, long elapsedMs)
    {
        Debug.Log($"멀티스레드 계산 완료 ({elapsedMs}ms)");

        if (path == null) { Debug.Log("경로 없음"); return; }

        var vpath = new List<Vector2Int>(path.Count);
        foreach (var p in path)
            vpath.Add(new Vector2Int(p.x, p.y));

        StopAllCoroutines();
        StartCoroutine(Follow(vpath));
    }

    IEnumerator Follow(List<Vector2Int> path)
    {
        foreach (var cell in path)
        {
            var target = grid.CellToWorldCenter(cell);
            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    void OnDestroy() => cts?.Cancel();

}
