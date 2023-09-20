namespace DBTest_integration;

using System.Collections;
using SimpleDB;

public class UnitTest1
{
    //source for test https://stackoverflow.com/questions/22093843/pass-complex-parameters-to-theory
    public record CheepRecordData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new Cheep[] {
                new Cheep("hanan","hej jj",1690978778)
            };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class CSVDatabase_IntegrationTest1
    {

        private readonly CSVDatabase<Cheep> _csvDatabase;

        public CSVDatabase_IntegrationTest1()
        {
            _csvDatabase = CSVDatabase<Cheep>.Instance;
        }



        #region Store_TestCode
        [Theory]
        [ClassData(typeof(CheepRecordData))]
        public void chek_that_database_contains_something(Cheep cheep)
        {
            var input = cheep;
            _csvDatabase.Store(input);

            Assert.NotEmpty(_csvDatabase.Read());
        }
        #endregion
    }
}