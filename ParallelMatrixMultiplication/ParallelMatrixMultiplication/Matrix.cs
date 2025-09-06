namespace ParallelMatrixMultiplication;

public class Matrix
{
    private int[][] data;

    private static int[] ParseLine(string line)
    {
        var splitLine = line.Split(" ");
        var result = new int[line.Length];
        for (var i = 0; i < line.Length; i++)
        {
            try
            {
                result[i] = int.Parse(splitLine[i]);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Неконсистентые данные в ячейках матрицы. Ошибка: {e.Message}");
                throw;
            }
        }

        return result;
    }

    public static Matrix FromFile(string filePath)
    {
        try
        {
            var lines = File.ReadAllLines(filePath);
            List<int[]> matrix = new();
            var lineSize = 0;
            foreach (var line in lines)
            {
                var vector = Matrix.ParseLine(line);
                if (vector.Length != lineSize)
                {
                    var message = $"Строки матрицы из файла {filePath} не соотносятся по разамерам.";
                    Console.Error.WriteLine(message);
                    throw new Exception(message);
                }
            }

            var resultMatrix = new Matrix();
            resultMatrix.data = matrix.ToArray();
            return resultMatrix;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(
                $"Не удалось проинициализировать матрицу из файла {filePath}. Возвращается пустой объект. Ошибка: {e.Message}");
            return new Matrix();
        }
    }

    public void ToFile(string filePath)
    {
        try
        {
            IEnumerable<string> lines =
                from row in data
                select string.Join(" ", row);
            File.WriteAllLines(filePath, lines);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Матрица не сохранена в {filePath}. Ошибка при сохранении матрицы: {e.Message}");
            throw;
        }
    }
}