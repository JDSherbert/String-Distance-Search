public static class StringDistanceSearch
{
    /// <summary>
    /// String distance search, where returned number is the total number of insertions, deletions and swaps
    /// </summary>
    /// <param name="searchText">The string being searching for</param>
    /// <param name="targetText">The string being searched in</param>
    /// <returns>int value for total number of insertions, deletions and swaps</returns>
    private static int DamerauLevenshteinDistance(string searchText, string targetText)
    {
        if (searchText.Equals(targetText)) return 0;
        if (string.IsNullOrEmpty(searchText) || string.IsNullOrEmpty(targetText)) return int.MaxValue;
            
        int matrixWidth = searchText.Length + 1;
        int matrixHeight = targetText.Length + 1;
        int[,] arrayD = new int[matrixWidth, matrixHeight];

        for (int i = 0; i < matrixWidth; i++) arrayD[i, 0] = i;
        for (int j = 0; j < matrixHeight; j++) arrayD[0, j] = j;
        for (int i = 1; i < matrixWidth; i++)
        {
            for (int j = 1; j < matrixHeight; j++)
            {
                int cost = searchText[i - 1] == targetText[j - 1] ? 0 : 1;

                // min from delete, insert and replacement operations
                int min = Mathf.Min(arrayD[i - 1, j] + 1, arrayD[i, j - 1] + 1, arrayD[i - 1, j - 1] + cost);
                arrayD[i, j] = min;

                if (i > 1 && j > 1 && searchText[i - 1] == targetText[j - 2] && searchText[i - 2] == targetText[j - 1])
                {
                    int minPermutation = Mathf.Min(arrayD[i, j], arrayD[i - 2, j - 2] + cost);
                    arrayD[i, j] = minPermutation;
                }
            }
        }

        return arrayD[matrixWidth - 1, matrixHeight - 1];
    }

    /// <summary>
    /// Credit: https://stackoverflow.com/a/9454016
    /// </summary>
    /// <param name="source">search query</param>
    /// <param name="target">text to search</param>
    /// <param name="threshold">how different can the strings be</param>
    /// <returns>int value for total number of insertions, deletions and swaps</returns>
    private static int DamerauLevenshteinDistanceShortCircuit(string source, string target, int threshold)
    {
        int length1 = source.Length;
        int length2 = target.Length;

        // Return trivial case - difference in string lengths exceeds threshold
        if (Math.Abs(length1 - length2) > threshold) return int.MaxValue;

        // Ensure arrays [i] / length1 use shorter length 
        if (length1 > length2)
        {
            (target, source) = (source, target);
            (length1, length2) = (length2, length1);
        }

        int maxI = length1;
        int maxJ = length2;

        int[] dCurrent = new int[maxI + 1];
        int[] dMinus1 = new int[maxI + 1];
        int[] dMinus2 = new int[maxI + 1];
        int[] dSwap;

        for (int i = 0; i <= maxI; i++) dCurrent[i] = i;

        int jm1 = 0;
        int im1 = 0;
        int im2 = -1;

        for (int j = 1; j <= maxJ; j++)
        {
            // Rotate
            dSwap = dMinus2;
            dMinus2 = dMinus1;
            dMinus1 = dCurrent;
            dCurrent = dSwap;

            // Initialize
            int minDistance = int.MaxValue;
            dCurrent[0] = j;
            im1 = 0;
            im2 = -1;

            for (int i = 1; i <= maxI; i++)
            {
                int cost = source[im1] == target[jm1] ? 0 : 1;
                int del = dCurrent[im1] + 1;
                int ins = dMinus1[i] + 1;
                int sub = dMinus1[im1] + cost;

                //Fastest execution for min value of 3 integers
                int min = (del > ins) ? (ins > sub ? sub : ins) : (del > sub ? sub : del);

                if (i > 1 && j > 1 && source[im2] == target[jm1] && source[im1] == target[j - 2])
                {
                    min = Math.Min(min, dMinus2[im2] + cost);
                }

                dCurrent[i] = min;
                if (min < minDistance) minDistance = min;

                im1++;
                im2++;
            }

            jm1++;
            if (minDistance > threshold) return int.MaxValue;
        }

        int result = dCurrent[maxI];
        return (result > threshold) ? int.MaxValue : result;
    }

    private static int DamerauLevenshteinDistanceSearch(string source, string target, int threshold)
    {
        if (threshold < 0) return DamerauLevenshteinDistance(source, target);
        else return DamerauLevenshteinDistanceShortCircuit(source, target, threshold);
    }
        
    public static int GetSearchDistance(string search, string target, int threshold = -1)
    {
        int minLength = Math.Min(search.Length, target.Length);
        int targetLength = target.Length;
        int difference = Math.Max(0, target.Length - search.Length);
            
        int minDistance = int.MaxValue;
            
        minDistance = Math.Min(minDistance, DamerauLevenshteinDistanceSearch(search, target.Substring(0, targetLength - difference), threshold));
        for (int i = 1; i <= difference; i++)
        {
            minDistance = Math.Min(minDistance, DamerauLevenshteinDistanceSearch(search, target.Substring(i, minLength), threshold));
        }
            
        return minDistance;
    }
}
