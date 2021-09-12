using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace SilverworkCodingChallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadJson();
        }
        public static void LoadJson()
        {
            //This will get the filepath of the project directory..
            string filePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\")) + @"loans.json";

            string _countryJson = File.ReadAllText(filePath);

            //here we will use Json.NET to read the provided json file and parse the data into a list of loan.cs class objects
            var _loans = JsonConvert.DeserializeObject<List<Loan>>(_countryJson);

            //now we will do the coding challenges. Challenge 1 is seperate things by state and get some totals and averages. 
            getMonthlySummary(_loans);
            
            //Challenge 2 is to do the same thing as challenge 1 but group it all by state...  
            getMonthlySummaryByState(_loans);
        }
        /// <summary>
        /// This will accept a list of loans and write a json file to the local file directory...it should contain the calculations across all states. It"s format should be as follows:
        /// </summary>
        /// <param name="list"></param>
        public static void getMonthlySummary(List<Loan> loanlist)
        {
            Dictionary<string, AmountSummary> monthlySummary = new Dictionary<string, AmountSummary>();

            var LoanAmountList = new List<double> { };
            var SubjectAppraisedAmountSummary = new List<double> { };
            var InterestRateSummary = new List<double> { };

            //parse each object into related lists...
            foreach (var x in loanlist)
            {
                LoanAmountList.Add(x.LoanAmount);
                SubjectAppraisedAmountSummary.Add(x.SubjectAppraisedAmount);
                InterestRateSummary.Add(x.InterestRate);
            }

            //do some calculations to get Sum, Average, Median, Minimum, Maximum from each list. Pack that stuff into classes to serialize into json later. 

            AmountSummary las = new AmountSummary();
            las.Sum = Math.Round(LoanAmountList.Sum(), 2);
            las.Average = Math.Round(LoanAmountList.Average(), 2);
            las.Median = Math.Round(LoanAmountList.Median(), 2);
            las.Minimum = Math.Round(LoanAmountList.Min(), 2);
            las.Maximum = Math.Round(LoanAmountList.Max(), 2);

            monthlySummary.Add("LoanAmountSummary", las);

            AmountSummary saas = new AmountSummary();
            saas.Sum = Math.Round(SubjectAppraisedAmountSummary.Sum(), 2);
            saas.Average = Math.Round(SubjectAppraisedAmountSummary.Average(), 2);
            saas.Median = Math.Round(SubjectAppraisedAmountSummary.Median(), 2);
            saas.Minimum = Math.Round(SubjectAppraisedAmountSummary.Min(), 2);
            saas.Maximum = Math.Round(SubjectAppraisedAmountSummary.Max(), 2);

            monthlySummary.Add("SubjectAppraisedAmountSummary", saas);

            AmountSummary irs = new AmountSummary();
            irs.Sum = Math.Round(InterestRateSummary.Sum(), 2);
            irs.Average = Math.Round(InterestRateSummary.Average(), 2);
            irs.Median = Math.Round(InterestRateSummary.Median(), 2);
            irs.Minimum = Math.Round(InterestRateSummary.Min(), 2);
            irs.Maximum = Math.Round(InterestRateSummary.Max(), 2);

            monthlySummary.Add("InterestRateSummary", irs);



            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(@"c:\monthlySummary.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, monthlySummary);
            }

        }
        /// <summary>
        /// This will get the requested summaries seperated by states... 
        /// </summary>
        /// <param name="loanlist"></param>
        public static void getMonthlySummaryByState(List<Loan> loanlist)
        {
            Dictionary<string, AmountSummaryList> statesList = new Dictionary<string, AmountSummaryList>();
           
            JsonSerializer serializer = new JsonSerializer();

            var loansGroupedByState = loanlist.GroupBy(x => x.SubjectState);

            foreach (var x in loansGroupedByState)
            {
                string state = x.Key;
                var LoanAmountList = new List<double> { };
                var SubjectAppraisedAmountSummary = new List<double> { };
                var InterestRateSummary = new List<double> { };
                foreach (var y in x)
                {
                    LoanAmountList.Add(y.LoanAmount);
                    SubjectAppraisedAmountSummary.Add(y.SubjectAppraisedAmount);
                    InterestRateSummary.Add(y.InterestRate);
                }

                //do some calculations to get Sum, Average, Median, Minimum, Maximum from each list. Pack that stuff into classes to serialize into json later. 

                LoanAmountSummary las = new LoanAmountSummary();
                las.Sum = Math.Round(LoanAmountList.Sum(), 2);
                las.Average = Math.Round(LoanAmountList.Average(), 2);
                las.Median = Math.Round(LoanAmountList.Median(), 2);
                las.Minimum = Math.Round(LoanAmountList.Min(), 2);
                las.Maximum = Math.Round(LoanAmountList.Max(), 2);

                SubjectAppraisedAmountSummary saas = new SubjectAppraisedAmountSummary();
                saas.Sum = Math.Round(SubjectAppraisedAmountSummary.Sum(), 2);
                saas.Average = Math.Round(SubjectAppraisedAmountSummary.Average(), 2);
                saas.Median = Math.Round(SubjectAppraisedAmountSummary.Median(), 2);
                saas.Minimum = Math.Round(SubjectAppraisedAmountSummary.Min(), 2);
                saas.Maximum = Math.Round(SubjectAppraisedAmountSummary.Max(), 2);

                InterestRateSummary irs = new InterestRateSummary();
                irs.Sum = Math.Round(InterestRateSummary.Sum(), 2);
                irs.Average = Math.Round(InterestRateSummary.Average(), 2);
                irs.Median = Math.Round(InterestRateSummary.Median(), 2);
                irs.Minimum = Math.Round(InterestRateSummary.Min(), 2);
                irs.Maximum = Math.Round(InterestRateSummary.Max(), 2);

                //Pack all this stuff into a class that can handle state abbreviations....
                AmountSummaryList amountbyState = new AmountSummaryList();
                amountbyState.loanAmountSummary = las;
                amountbyState.subjectAppraisedAmountSummary = saas;
                amountbyState.InterestRateSummary = irs;

                statesList.Add(state, amountbyState);

            }

           
            using (StreamWriter sw = new StreamWriter(@"c:\monthlyByState.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, statesList);
            }
           
            
        }
        public class AmountSummaryList
        {
            
            public LoanAmountSummary loanAmountSummary { get; set; }
            public SubjectAppraisedAmountSummary subjectAppraisedAmountSummary { get; set; }
            public InterestRateSummary InterestRateSummary { get; set; }
        }
        public class AmountSummary
        {
            public double Sum { get; set; }
            public double Average { get; set; }
            public double Median { get; set; }
            public double Minimum { get; set; }
            public double Maximum { get; set; }
        }
        public class LoanAmountSummary
        {
            public double Sum { get; set; }
            public double Average { get; set; }
            public double Median { get; set; }
            public double Minimum { get; set; }
            public double Maximum { get; set; }
        }
        public class SubjectAppraisedAmountSummary
        {
            public double Sum { get; set; }
            public double Average { get; set; }
            public double Median { get; set; }
            public double Minimum { get; set; }
            public double Maximum { get; set; }
        }
        public class InterestRateSummary
        {
            public double Sum { get; set; }
            public double Average { get; set; }
            public double Median { get; set; }
            public double Minimum { get; set; }
            public double Maximum { get; set; }
        }

    }
}
