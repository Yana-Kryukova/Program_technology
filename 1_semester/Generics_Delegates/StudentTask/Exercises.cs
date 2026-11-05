namespace Practice1;

/// <summary>
/// Задание на 15 минут. Замените каждый throw рабочим кодом.
/// Проверка:  dotnet test Tests     (из папки Practice1)
/// Выполнено, когда ВСЕ тесты зелёные. Что не успели, доделайте дома.
/// </summary>
public static class Exercises
{
    // ГОТОВО, не менять. Меняет соседей местами, если shouldSwap(левый, правый) == true.
    public static void BubbleSort<T>(T[] arr, Func<T, T, bool> shouldSwap)
    {
        for (int i = 0; i < arr.Length - 1; i++)
            for (int j = 0; j < arr.Length - 1 - i; j++)
                if (shouldSwap(arr[j], arr[j + 1]))
                    (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
    }

    // ============ ЧАСТЬ A. Повторяем на сортировке (5 минут) ============

    // A1. Отсортировать числа по УБЫВАНИЮ (500, 20, 3). Вызовите BubbleSort с лямбдой.
    public static void SortDescending(int[] arr)
    {
        throw new NotImplementedException();
    }

    // A2. Отсортировать строки по ДЛИНЕ, короткие первыми ("я", "кот", "слон").
    //     Подсказка: у строки есть свойство Length.
    public static void SortByLength(string[] arr)
    {
        throw new NotImplementedException();
    }

    // ============ ЧАСТЬ B. Новая задача (10 минут) ============
    // Здесь сортировки нет, но приём тот же: универсальный метод + правило-делегат.

    // B1. Сколько элементов массива удовлетворяют условию?
    //     CountIf(new[] {1, 8, 3, 10}, x => x > 5)  ->  2
    public static int CountIf<T>(T[] arr, Func<T, bool> condition)
    {
        throw new NotImplementedException();
    }

    // B2. Вернуть НОВЫЙ массив только из подходящих элементов, порядок сохранить.
    //     Исходный массив менять нельзя.
    //     Filter(new[] {1, 8, 3, 10}, x => x > 5)  ->  { 8, 10 }
    //     Подсказка: соберите результат в List<T>, в конце вызовите ToArray().
    public static T[] Filter<T>(T[] arr, Func<T, bool> condition)
    {
        throw new NotImplementedException();
    }
}
