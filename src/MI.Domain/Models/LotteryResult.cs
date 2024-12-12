namespace MI.Domain.Models;

public class LotteryResult
{
    public LotteryResult(DateOnly drawDate, int contestId, decimal accumulated, IReadOnlyList<int> results)
    {
        DrawDate = drawDate;
        ContestId = contestId;
        Accumulated = accumulated;
        InsertResults(results);
    }

    protected LotteryResult()
    {
    }

    public long Id { get; }
    public DateOnly DrawDate { get; private set; }
    public int ContestId { get; private set; }
    public decimal Accumulated { get; private set; }
    public int Result01 { get; private set; }
    public int Result02 { get; private set; }
    public int Result03 { get; private set; }
    public int Result04 { get; private set; }
    public int Result05 { get; private set; }
    public int Result06 { get; private set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    private void InsertResults(IReadOnlyList<int> results)
    {
        if (results.Count == 0)
        {
            throw new NullReferenceException();
        }

        Result01 = results[0];
        Result02 = results[1];
        Result03 = results[2];
        Result04 = results[3];
        Result05 = results[4];
        Result06 = results[5];
    }
}