using Xunit;

namespace Practice1.Tests;

public class PartA_Sorting
{
    [Fact]
    public void SortDescending_SortsNumbers()
    {
        var arr = new[] { 3, 500, 20, 7 };
        Exercises.SortDescending(arr);
        Assert.Equal(new[] { 500, 20, 7, 3 }, arr);
    }

    [Fact]
    public void SortDescending_HandlesNegativesAndDuplicates()
    {
        var arr = new[] { -1, 5, 5, -10, 0 };
        Exercises.SortDescending(arr);
        Assert.Equal(new[] { 5, 5, 0, -1, -10 }, arr);
    }

    [Fact]
    public void SortDescending_EmptyAndSingle()
    {
        var empty = Array.Empty<int>();
        Exercises.SortDescending(empty);
        Assert.Empty(empty);

        var one = new[] { 9 };
        Exercises.SortDescending(one);
        Assert.Equal(new[] { 9 }, one);
    }

    [Fact]
    public void SortByLength_ShortFirst()
    {
        var arr = new[] { "слон", "я", "ананас", "кот" };
        Exercises.SortByLength(arr);
        Assert.Equal(new[] { "я", "кот", "слон", "ананас" }, arr);
    }

    [Fact]
    public void SortByLength_EmptyArray()
    {
        var arr = Array.Empty<string>();
        Exercises.SortByLength(arr);
        Assert.Empty(arr);
    }
}

public class PartB_CountIf
{
    [Fact]
    public void CountIf_Ints()
        => Assert.Equal(2, Exercises.CountIf(new[] { 1, 8, 3, 10 }, x => x > 5));

    [Fact]
    public void CountIf_Strings()
        => Assert.Equal(2, Exercises.CountIf(new[] { "кот", "слон", "ёж", "лев" }, s => s.Length == 3));

    [Fact]
    public void CountIf_NothingMatches_ReturnsZero()
        => Assert.Equal(0, Exercises.CountIf(new[] { 1, 2, 3 }, x => x > 100));

    [Fact]
    public void CountIf_EmptyArray_ReturnsZero()
        => Assert.Equal(0, Exercises.CountIf(Array.Empty<int>(), x => true));

    [Fact]
    public void CountIf_ConditionUsesOuterVariable()
    {
        int limit = 2;
        var arr = new[] { 1, 2, 3, 4 };
        Assert.Equal(2, Exercises.CountIf(arr, x => x > limit));
        limit = 0;                       // замыкание видит новое значение
        Assert.Equal(4, Exercises.CountIf(arr, x => x > limit));
    }
}

public class PartB_Filter
{
    [Fact]
    public void Filter_Ints_KeepsOrder()
        => Assert.Equal(new[] { 8, 10 }, Exercises.Filter(new[] { 1, 8, 3, 10 }, x => x > 5));

    [Fact]
    public void Filter_Strings()
        => Assert.Equal(new[] { "кот", "лев" },
            Exercises.Filter(new[] { "кот", "слон", "лев" }, s => s.Length == 3));

    [Fact]
    public void Filter_NothingMatches_ReturnsEmptyArray()
        => Assert.Empty(Exercises.Filter(new[] { 1, 2, 3 }, x => x > 100));

    [Fact]
    public void Filter_DoesNotChangeSource()
    {
        var src = new[] { 1, 8, 3, 10 };
        var result = Exercises.Filter(src, x => x > 5);
        Assert.Equal(new[] { 1, 8, 3, 10 }, src);
        Assert.NotSame(src, result);
    }

    [Fact]
    public void Filter_EmptyArray()
        => Assert.Empty(Exercises.Filter(Array.Empty<int>(), x => true));
}
