using System;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml;
using System.Xml.XPath;

namespace CourseEditor
{
    public partial class Timetable : Form
    {
        public XDocument document;
        public Timetable()
        {
            InitializeComponent();
        }

        private void ClearLabels()
        {
            this.authorLabel.Text = "AUTHOR";
            this.numLabel.Text = "No";
            this.semLabel.Text = "SEMESTER";
            this.nameLabel.Text = "NAME";
            this.idLabel.Text = "ID";
            this.ectsLabel.Text = "ECTS";
            this.lecLabel.Text = "LECTURE H";
            this.tutLabel.Text = "TUTORIAL H";
            this.labLabel.Text = "LABORATORY H";
            this.dateLabel.Text = "DATE GRADED";
            this.weightLabel.Text = "WEIGHT";
        }

        private void DisplayCourses()
        {
            XNamespace ns = "timetable.pl";
            var baseNamespace = new XmlNamespaceManager(new NameTable());
            baseNamespace.AddNamespace("n", "timetable.pl");

            this.authorLabel.Text = "author: " + document.XPathSelectElements("/n:COURSES_LIST/n:READ_ME/n:AUTHOR", baseNamespace).First().Value
                + ",     id: " + document.XPathSelectElements("/n:COURSES_LIST/n:READ_ME/n:INDEX", baseNamespace).First().Value;

            int i = 0;
            foreach (XElement el in document.XPathSelectElements("/n:COURSES_LIST/n:COURSES/n:COURSE", baseNamespace))
            {
                this.numLabel.Text += "\n" + document.Element(ns + "COURSES_LIST").Element(ns + "COURSES").Descendants(ns + "COURSE").ElementAt(i).Attribute("nr").Value.Substring(1);

                this.semLabel.Text += "\n" + document.Element(ns + "COURSES_LIST").Element(ns + "COURSES").Descendants(ns + "COURSE").ElementAt(i).Attribute("semID").Value.Substring(1);

                this.nameLabel.Text += "\n"+ document.XPathSelectElements("/n:COURSES_LIST/n:COURSES/n:COURSE/n:NAME | /n:COURSES_LIST/n:COURSES/n:COURSE/n:POLISH_NAME", baseNamespace).ElementAt(i).Value;

                this.idLabel.Text += "\n" + document.XPathSelectElements("/n:COURSES_LIST/n:COURSES/n:COURSE/n:ID", baseNamespace).ElementAt(i).Value;

                this.ectsLabel.Text += "\n" + document.XPathSelectElements("/n:COURSES_LIST/n:COURSES/n:COURSE/n:ECTS", baseNamespace).ElementAt(i).Value;

                this.lecLabel.Text += "\n" + document.XPathSelectElements("/n:COURSES_LIST/n:COURSES/n:COURSE/n:LECTURE_H", baseNamespace).ElementAt(i).Value;

                this.tutLabel.Text += "\n" + document.XPathSelectElements("/n:COURSES_LIST/n:COURSES/n:COURSE/n:TUTORIAL_H", baseNamespace).ElementAt(i).Value;

                this.labLabel.Text += "\n" + document.XPathSelectElements("/n:COURSES_LIST/n:COURSES/n:COURSE/n:LABORATORY_H", baseNamespace).ElementAt(i).Value;

                this.dateLabel.Text += "\n" + document.XPathSelectElements("/n:COURSES_LIST/n:COURSES/n:COURSE/n:GRADING_DATE", baseNamespace).ElementAt(i).Value;

                this.weightLabel.Text += "\n" + document.XPathSelectElements("/n:COURSES_LIST/n:COURSES/n:COURSE/n:WEIGHT", baseNamespace).ElementAt(i).Value;
                
                i++;
                
            }
        }

        private bool ValidateDocument(XDocument target)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null, "format.xsd");
            schemas.Add(null, "types.xsd");


            try { target.Validate(schemas, null); }
            catch (XmlSchemaException xsd)
            {
                return false;
            }
            return true;
        }

        private bool AddCourse()
        {
            XDocument updated = new XDocument(document);

            int number0 = int.Parse(this.numInput.Text);
            string sem0 = this.semInput.Text;
            string name0 = this.nameInput.Text;
            string id0 = this.idInput.Text;
            int ects0 = int.Parse(this.ectsInput.Text);
            int lec0 = int.Parse(this.lecInput.Text);
            int tut0 = int.Parse(this.tutInput.Text);
            int lab0 = int.Parse(this.labInput.Text);
            string date0 = this.dateInput.Text;
            double w = ects0 / 30.0;
            string weight0 = w.ToString("F2");

            XNamespace ns = "timetable.pl";

            //string n0 = document.Element(ns + "COURSES_LIST").Element(ns + "COURSES").Descendants(ns + "COURSE").Last().Attribute("nr").Value;
            

            updated.Element(ns + "COURSES_LIST").Element(ns + "COURSES").Add(
            new XElement(ns + "COURSE",
                new XAttribute("nr", "C" + number0),
                new XAttribute("semID", "S" + sem0),
                new XElement(ns + "NAME", name0),
                new XElement(ns + "ID", id0),
                new XElement(ns + "ECTS", ects0),
                new XElement(ns + "LECTURE_H", lec0),
                new XElement(ns + "TUTORIAL_H", tut0),
                new XElement(ns + "LABORATORY_H", lab0),
                new XElement(ns + "GRADING_DATE", new XAttribute("graded", true), date0),
                new XElement(ns + "WEIGHT", weight0)
                ));

            if (ValidateDocument(updated))
            {
                document = updated;
                return true;
            }

            return false;
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            string filename = this.fileInput.Text;
            document = XDocument.Load(filename);

            if (ValidateDocument(document))
            {
                ClearLabels();
                DisplayCourses();
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (AddCourse())
            {
                ClearLabels();
                DisplayCourses();
            }
            
        }


    }
}