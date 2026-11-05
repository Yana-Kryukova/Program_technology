using Demo;

// =====================================================================
// Практика 1. Делегаты, лямбды и generics на примере сортировки.
// =====================================================================

int[] Fresh() => new[] { 42, 7, 15, 3, 28 };

Console.WriteLine("=== ШАГ 1. Сортировка по возрастанию ===");
var a = Fresh();
Sorting.SortAscending(a);
Console.WriteLine(string.Join(", ", a));          // 3, 7, 15, 28, 42

Console.WriteLine();
Console.WriteLine("=== ШАГ 2. Нужны ещё правила: копипаста ===");
var b = Fresh();
Sorting.SortDescending(b);
Console.WriteLine("по убыванию:        " + string.Join(", ", b));
var c = Fresh();
Sorting.SortByLastDigit(c);
Console.WriteLine("по последней цифре: " + string.Join(", ", c));
// LIVE 2-3: вынести условие в параметр-делегат, использовать лямбды.

Console.WriteLine();
Console.WriteLine("=== ШАГ 3. А строки? ===");
string[] words = { "ананас", "кот", "слон", "я" };
// Sorting.SortAscending(words);   // не скомпилируется: метод только для int[]
Console.WriteLine("Хотим отсортировать строки по длине, но методы знают только int[].");
// LIVE 4: сделать метод generic: BubbleSort<T>.
