using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;


public class NativeGrid2DBatch<T> where T : unmanaged
{
    public readonly int Width;
    public readonly int Height;
    
    private NativeArray<T> batch1;
    private NativeArray<T> batch2;

    private int cBatchId;

    public ref NativeArray<T> CurrentBatch => ref (cBatchId == 0 ? ref batch1 : ref batch2);
    public ref NativeArray<T> NextBatch => ref (cBatchId == 0 ? ref batch2 : ref batch1);


    public NativeGrid2DBatch(int width, int height, Allocator allocator = Allocator.Persistent)
    {
        Width = width;
        Height = height;

        batch1 = new NativeArray<T>(width * height, allocator);
        batch2 = new NativeArray<T>(width * height, allocator);
    }

    public T this[int2 gridPos]
    {
        get => CurrentBatch[gridPos.x + gridPos.y * Width];
        set => CurrentBatch[gridPos.x + gridPos.y * Width] = value;
    }
    public T this[int gridId]
    {
        get => CurrentBatch[gridId];
        set => CurrentBatch[gridId] = value;
    }

    public int GetGridId(int2 gridPos)
    {
        return gridPos.x + gridPos.y * Width;
    }



    public void CycleToNextBatch()
    {
        // Create and Instantly complete Copy Job
        // Copy new array into old array
        // Afterwards "old" arraybecomes new array
        new ArrayCopyJob<T>()
        {
            destination = CurrentBatch,
            source = NextBatch,
        }.Run();

        cBatchId ^= 1; // Flip between 0 and 1
    }

    public void Dispose()
    {
        batch1.DisposeIfCreated();
        batch2.DisposeIfCreated();
    }
}