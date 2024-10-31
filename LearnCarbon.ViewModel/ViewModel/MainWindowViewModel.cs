using LearnCarbon.Helper.Helper;
using Rhino;
using Rhino.DocObjects;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using System.Collections.ObjectModel;
using System.IO;

namespace LearnCarbon.ViewModel.ViewModel
{
    public class MainWindowViewModel : BaseModel
    {
        RhinoDoc doc;
        double height;
        List<ObjRef> selectedObjects;

        // Temp to setup
        private readonly string pythonScriptPath;
        private readonly string resultFilePath;
        private readonly string inputFilePath;

        #region Properties

        #region Selection
        private double footprint;

        public double Footprint
        {
            get { return footprint; }
            set
            {
                footprint = Math.Round(value, 3);

                TotalArea = Math.Round(value * NoOfFloor, 3);
                OnPropertyChange();
            }
        }

        private int noOfFloor = 1;

        public int NoOfFloor
        {
            get { return noOfFloor; }
            set
            {
                noOfFloor = value;
                TotalArea = Math.Round(value * Footprint, 3);
                OnPropertyChange();
            }
        }
        private double typicalHeight = 10;

        public double TypicalHeight
        {
            get { return typicalHeight; }
            set
            {
                typicalHeight = value;
                NoOfFloor = (int)Math.Ceiling(height / typicalHeight);
                OnPropertyChange();
            }
        }

        private double totalArea;
        public double TotalArea
        {
            get { return totalArea; }
            set { totalArea = value; OnPropertyChange(); }
        }

        public ObservableCollection<string> Locations { get; set; }

        private string location;
        public string Location
        {
            get { return location; }
            set { location = value; OnPropertyChange(); }
        }

        private bool isCommercial;

        public bool IsCommercial
        {
            get { return isCommercial; }
            set { isCommercial = value; OnPropertyChange(); }
        }
        private bool isResidential;

        public bool IsResidential
        {
            get { return isResidential; }
            set { isResidential = value; OnPropertyChange(); }
        }
        #endregion

        #region Analysis

        #region OptionA
        private bool optionA;

        public bool OptionA
        {
            get { return optionA; }
            set { optionA = value; OnPropertyChange(); }
        }
        private bool isTimber;

        public bool IsTimber
        {
            get { return isTimber; }
            set { isTimber = value; OnPropertyChange(); }
        }

        private bool isConcreteTimber;

        public bool IsConcreteTimber
        {
            get { return isConcreteTimber; }
            set { isConcreteTimber = value; OnPropertyChange(); }
        }

        private bool isConcrete;

        public bool IsConcrete
        {
            get { return isConcrete; }
            set { isConcrete = value; OnPropertyChange(); }
        }

        private bool isSteelConcrete;

        public bool IsSteelConcrete
        {
            get { return isSteelConcrete; }
            set { isSteelConcrete = value; OnPropertyChange(); }
        }

        //private bool isSteel;

        //public bool IsSteel
        //{
        //    get { return isSteel; }
        //    set { isSteel = value; OnPropertyChange(); }
        //}

        #endregion

        #region OptionB
        private bool optionB;

        public bool OptionB
        {
            get { return optionB; }
            set { optionB = value; OnPropertyChange(); }
        }

        private double targetCo2;

        public double TargetCo2
        {
            get { return targetCo2; }
            set { targetCo2 = value; OnPropertyChange(); }
        }

        #endregion

        #endregion

        #region Output
        private string output;

        public string Output
        {
            get { return output; }
            set { output = value; OnPropertyChange(); }
        }

        private bool isConWoodHyb;
        public bool IsConWoodHyb
        {
            get { return isConWoodHyb; }
            set { isConWoodHyb = value; OnPropertyChange(); }
        }

        private bool isWood;
        public bool IsWood
        {
            get { return isWood; }
            set { isWood = value; OnPropertyChange(); }
        }

        private bool isSteConHyb;
        public bool IsSteConHyb
        {
            get { return isSteConHyb; }
            set { isSteConHyb = value; OnPropertyChange(); }
        }

        private bool isRc;
        public bool IsRc
        {
            get { return isRc; }
            set { isRc = value; OnPropertyChange(); }
        }
        #endregion

        #endregion

        #region Commands
        public RelayCommand SelectCommand { get; set; }
        public RelayCommand CalculateCommand { get; set; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            doc = RhinoDoc.ActiveDoc;
            Locations = new ObservableCollection<string>()
        {
            "Africa","Asia-Pacific","Europe","Middle East","North America","South Americal"
        };
            Location = Locations[0];
            CalculateCommand = new RelayCommand(Calculate, null);
            SelectCommand = new RelayCommand(SelectModelObj, null);
            RhinoDoc.ReplaceRhinoObject += Doc_ReplaceRhinoObject;

            string ProjectDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            // Move up to the main repository folder
            string repoDirectory = Path.GetFullPath(Path.Combine(ProjectDirectory, @"..\..\..\.."));
            // Navigate to the 'src' folder and then to 'run_ml.py'
            pythonScriptPath = Path.Combine(repoDirectory, "src", "run_ml.py");
            resultFilePath = Path.Combine(repoDirectory, "src", "log", "prediction_result.txt");
            inputFilePath = Path.Combine(repoDirectory, "src", "log", "input.txt");


            if (File.Exists(pythonScriptPath))
            {
                Console.WriteLine("Python script found at: " + pythonScriptPath);
            }
            else
            {
                Console.WriteLine("Python script not found.");
            }
            double debug = 0;
        }
        #endregion

        #region Command Methods
        private void SelectModelObj()
        {
            selectedObjects = new List<ObjRef>();

            selectedObjects = GetRhinoObjects();

            if (selectedObjects == null) return;

            List<BoundingBox> boxList = new List<BoundingBox>();
            Footprint = 0.0;
            for (int i = 0; i < selectedObjects.Count; i++)
            {
                var obj = selectedObjects[i];

                Brep brep = obj.Brep();

                if (brep != null)
                {
                    boxList.Add(brep.GetBoundingBox(false));

                    Footprint += GetFloorArea(brep);
                }
            }

            SetHeight(boxList);
        }
        private void Calculate()
        {
            try
            {
                // TO DO run this for every selected object and create a parallel list with Co2
                RunPythonScript(optionA == true);
                //RunPythonScriptDEBUG();

            }
            catch (Exception e)
            {
                Output = e.Message;
            }
        }

        public void RunPythonScript(bool optionA)
        {
            try
            {
                // Write inputs to the input file
                using (StreamWriter sw = new StreamWriter(inputFilePath))
                {
                    int buildingType = IsResidential ? 1 : 0;

                    int location = 0;
                    switch (Location)
                    {
                        case "Asia-Pacific":
                            location = 1;
                            break;
                        case "Europe":
                            location = 2;
                            break;
                        case "Middle East":
                            location = 3;
                            break;
                        case "North America":
                            location = 4;
                            break;
                        default:
                            location = 5;
                            break;
                    }

                    if (optionA)
                    {
                        int constructionType = 0;
                        if (IsSteelConcrete) constructionType = 1;
                        else if (IsTimber) constructionType = 2;
                        else if (IsConcreteTimber) constructionType = 3;

                        // Write modelA inputs to file
                        sw.WriteLine($"{optionA},{constructionType},{buildingType},{location},{TotalArea},{noOfFloor}");
                    }
                    else
                    {
                        // Write modelB input to file
                        sw.WriteLine($"{optionA},{targetCo2},{buildingType},{location},{TotalArea},{noOfFloor}");
                    }
                }

                string rhinoScriptCommand = $"-_ScriptEditor _Run \"{pythonScriptPath}\"";
                bool debug = RhinoApp.RunScript(rhinoScriptCommand, false);

                // Read the result from the result file
                string prediction = System.IO.File.ReadAllText(resultFilePath);

                if(optionA)
                    Output = $"{prediction} tCo2e";
                else
                    Output = prediction;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Error: {ex.Message}");
            }
        }
        #endregion

        #region private methods
        private List<ObjRef> GetRhinoObjects()
        {
            const ObjectType geometryFilter = ObjectType.Brep;
            GetObject go = new GetObject();
            go.SetCommandPrompt("Select building objects - Brep or meshes");
            go.GeometryFilter = geometryFilter;
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;

            bool bHavePreselectedObjects = false;

            for (; ; )
            {
                GetResult res = go.GetMultiple(1, 0);

                if (res == GetResult.Option)
                {
                    go.EnablePreSelect(false, true);
                    continue;
                }

                else if (res != GetResult.Object)
                    return null;

                if (go.ObjectsWerePreselected)
                {
                    bHavePreselectedObjects = true;
                    go.EnablePreSelect(false, true);
                    continue;
                }
                break;
            }

            if (bHavePreselectedObjects)
            {
                // Normally, pre-selected objects will remain selected, when a
                // command finishes, and post-selected objects will be unselected.
                // This this way of picking, it is possible to have a combination
                // of pre-selected and post-selected. So, to make sure everything
                // "looks the same", lets unselect everything before finishing
                // the command.
                for (int i = 0; i < go.ObjectCount; i++)
                {
                    RhinoObject rhinoObject = go.Object(i).Object();
                    if (null != rhinoObject)
                        rhinoObject.Select(false);
                }
                doc.Views.Redraw();
            }

            int objectCount = go.ObjectCount;


            RhinoApp.WriteLine(string.Format("Selected object count = {0}", objectCount));


            var selectedObjects = go.Objects().ToList();
            return selectedObjects;
        }
        private double GetFloorArea(Brep brep)
        {
            if (brep == null) return 0;
            var faces = brep.Faces;
            foreach (var face in faces)
            {
                var pt = AreaMassProperties.Compute(face).Centroid;
                face.ClosestPoint(pt, out double u, out double v);

                var vectorN = face.NormalAt(u, v);
                if (vectorN.IsParallelTo(new Vector3d(0, 0, -1)) == 1)
                {
                    return AreaMassProperties.Compute(face).Area;
                }
            }
            return 0;
        }

        private void SetHeight(List<BoundingBox> boundingBoxes)
        {
            var bb = boundingBoxes[0];
            for (int j = 1; j < boundingBoxes.Count; j++)
            {
                bb = BoundingBox.Union(bb, boundingBoxes[j]);
            }
            height = bb.Max.Z - bb.Min.Z;
            NoOfFloor = (int)Math.Ceiling(height / TypicalHeight);
        }
        #endregion

        #region Event methods
        private void Doc_ReplaceRhinoObject(object sender, RhinoReplaceObjectEventArgs e)
        {
            var boxList = new List<BoundingBox>();

            for (int i = 0; i < selectedObjects.Count; i++)
            {
                if (selectedObjects[i].ObjectId == e.ObjectId)
                {
                    if (e.OldRhinoObject.Geometry.HasBrepForm)
                    {
                        var brep1 = Brep.TryConvertBrep(e.OldRhinoObject.Geometry);
                        Footprint -= GetFloorArea(brep1);

                        var brep2 = Brep.TryConvertBrep(e.NewRhinoObject.Geometry);
                        Footprint += GetFloorArea(brep2);
                        boxList.Add(brep2.GetBoundingBox(false));
                    }
                }
                else
                {
                    boxList.Add(selectedObjects[i].Brep().GetBoundingBox(false));
                }
            }
            SetHeight(boxList);
        }
        #endregion
    }
}

