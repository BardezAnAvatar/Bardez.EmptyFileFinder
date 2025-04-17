// See https://aka.ms/new-console-template for more information
using Bardez.EmptyFileFinder;

Console.WriteLine("Checking for NUL-only files...");

//await Checker.CheckForEmptyFiles(CancellationToken.None);
await Checker.CheckForEmptyFiles("\\\\bardezserver2\\Storage\\Games\\Infinity & Aurora Engine\\", CancellationToken.None);