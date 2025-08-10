using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CalculationModel.Services
{
    public class AllowanceService
    {
        private readonly int[] Diameters = new[]
        {
        500, 630, 800, 1000, 1250, 1400, 1600, 1800, 2000,
        2250, 2500, 2800, 3150, 3500, 4000, 4500, 5000
        };

        private readonly int[] Heights = new[]
        {
        150, 200, 250, 315, 400, 500, 630, 800, 1000,
        1250, 1400, 1600, 1800, 2000, 2250, 2500
        };

        private readonly (int allowance, int delta)[,] Allowances = new (int, int)[,]
        {
            { (24, 9),  (25, 9),  (27, 10), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
            { (24, 9),  (25, 9),  (27, 10), (29, 11), (31, 11), (35, 13), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
            { (25, 9),  (26, 9),  (28, 10), (30, 11), (32, 12), (36, 13), (38, 14), (40, 15), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
            { (27, 10), (28, 10), (30, 11), (32, 12), (34, 13), (38, 14), (41, 15), (44, 16), (47, 18), (51, 19), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
            { (28, 10), (29, 11), (31, 11), (33, 12), (35, 13), (40, 15), (42, 16), (46, 17), (49, 19), (53, 20), (57, 21), (61, 23), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) },
            { (29, 11), (30, 11), (31, 11), (34, 13), (36, 13), (41, 15), (44, 16), (48, 18), (51, 19), (55, 21), (59, 22), (63, 24), (67, 25), (71, 27), (0, 0), (0, 0), (0, 0) },
            { (30, 11), (31, 11), (33, 12), (35, 13), (37, 14), (43, 16), (46, 17), (50, 19), (53, 20), (57, 21), (61, 23), (65, 25), (69, 26), (75, 28), (80, 30), (95, 37), (0, 0) },
            { (0, 0),   (33, 12), (36, 13), (41, 15), (46, 17), (50, 19), (54, 20), (57, 21), (61, 23), (65, 25), (71, 27), (77, 29), (83, 31), (89, 34), (98, 39), (105, 43), (0, 0) },
            { (0, 0),   (0, 0),   (37, 14), (40, 15), (43, 16), (48, 18), (52, 20), (56, 21), (59, 22), (63, 24), (68, 26), (74, 28), (80, 30), (86, 33), (92, 36), (101, 41), (108, 46) },
            { (0, 0),   (0, 0),   (0, 0),   (44, 16), (46, 17), (52, 20), (56, 21), (60, 23), (64, 24), (68, 26), (74, 28), (81, 30), (86, 33), (92, 36), (98, 39), (105, 43), (112, 48) },
            { (0, 0),   (0, 0),   (0, 0),   (0, 0),   (47, 18), (54, 20), (58, 22), (62, 23), (67, 25), (72, 27), (78, 29), (83, 31), (89, 34), (95, 37), (101, 41), (108, 45), (115, 50) },
            { (0, 0),   (0, 0),   (0, 0),   (0, 0),   (48, 18), (56, 21), (60, 23), (65, 24), (69, 26), (75, 28), (81, 30), (87, 34), (93, 36), (99, 39), (104, 42), (110, 47), (119, 51) },
            { (0, 0),   (0, 0),   (0, 0),   (0, 0),   (0, 0),   (58, 22), (63, 23), (67, 25), (73, 27), (79, 29), (85, 33), (91, 35), (97, 38), (102, 41), (106, 43), (113, 48), (120, 52) },
            { (0, 0),   (0, 0),   (0, 0),   (0, 0),   (0, 0),   (0, 0),   (64, 23), (68, 25), (75, 28), (81, 30), (87, 34), (92, 36), (98, 38), (103, 41), (107, 43), (115, 50), (121, 52) },
            { (0, 0),   (0, 0),   (0, 0),   (0, 0),   (0, 0),   (0, 0),   (0, 0),   (69, 26), (76, 28), (82, 30), (88, 34), (94, 37), (100, 40), (104, 42), (108, 43), (116, 50), (122, 52) },
            { (0, 0),   (0, 0),   (0, 0),   (0, 0),   (0, 0),   (0, 0),   (0, 0),   (0, 0),   (82, 30), (85, 32), (91, 35), (97, 38), (100, 41), (108, 43), (114, 49), (119, 51), (125, 53) },
        };

        public (int, int) FindAllowanceAndDelta(int diameter, int height)
        {
            try
            {
                if (diameter <= 0 || height <= 0)
                    throw new ArgumentException("Diameter and height must be positive.");

                int heightIndex = Array.FindIndex(Heights, h => height <= h);
                if (heightIndex == -1)
                    heightIndex = Heights.Length - 1;

                int diameterIndex = Array.FindIndex(Diameters, d => diameter <= d);
                if (diameterIndex == -1)
                    diameterIndex = Diameters.Length - 1;

                if (heightIndex < 0 || heightIndex >= Allowances.GetLength(0))
                    throw new IndexOutOfRangeException($"Height index {heightIndex} is out of range for allowances table.");
                if (diameterIndex < 0 || diameterIndex >= Allowances.GetLength(1))
                    throw new IndexOutOfRangeException($"Diameter index {diameterIndex} is out of range for allowances table.");

                var result = Allowances[heightIndex, diameterIndex];


                return result;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new InvalidOperationException($"Internal table indexing error. Check diameter and height ranges.", ex);
            }
            catch (Exception ex) when (!(ex is ArgumentException || ex is InvalidOperationException))
            {
                throw new InvalidOperationException($"An unexpected error occurred while calculating allowance.", ex);
            }
        }
    }
}
