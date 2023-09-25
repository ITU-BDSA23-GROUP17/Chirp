namespace E2ETest;

using System.Diagnostics;

public class EndToEndTest1
{
    [Fact]
    public void end2endtest1()
    {
        //Arrange
        var directory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
        string workingDirectory = Path.Combine(directory, "src/Chirp.CLI.Client/bin/Debug/net7.0/");

        //Defines the process start info for running the application for the test
        var chirpTestStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "Chirp.CLI.dll read",
            RedirectStandardOutput = true, //We need to redirect output in order to capture it and compare with expected
            WorkingDirectory = workingDirectory
        };

        //Define expected output for test
        var expectedOutput = @"ropf @ 8/1/2023 12:09:20 PM: Hello, BDSA students! 
        rnie @ 8/2/2023 12:19:38 PM: Welcome to the course! 
        rnie @ 8/2/2023 12:37:38 PM: I hope you had a good summer. 
        ropf @ 8/2/2023 1:04:47 PM: Cheeping cheeps on Chirp :)";

        //Act
        //Start new process with specified start info
        using (var process = new Process { StartInfo = chirpTestStartInfo })
        {
            process.Start();
            process.WaitForExit();

            //read actual output
            var output = process.StandardOutput.ReadToEnd().Trim();
            Console.WriteLine(output);

            //We need to fix line endings so they are the same, using '\n' in both expected and actual outputs before assert comparison.
            expectedOutput = expectedOutput.Replace("        ", "");
            output = output.Replace("\r\n", "\n");
            Console.WriteLine("Expected Output:");
            Console.WriteLine(expectedOutput);
            Console.WriteLine("Actual Output:");
            Console.WriteLine(output);

            //Assert
            //We verify that the actual and expected outputs are equal.
            Assert.Equal(expectedOutput, output);
        }
    }
}

