using Grasshopper.Kernel;
using GWPEstimator.Helper.Helper;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using System.Collections.ObjectModel;
using System.IO;

namespace GWPEstimator.ViewModel.ViewModel
{
    public class MainWindowViewModel : BaseModel
    {
        RhinoDoc doc;
        GH_Document GrasshopperDocument;
        double height;
        List<ObjRef> selectedObjects;

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
            //var bb = boxList[0];
            //for (int j = 1; j < boxList.Count; j++)
            //{
            //    bb = BoundingBox.Union(bb, boxList[j]);
            //}
            //height = bb.Max.Z - bb.Min.Z;
            //NoOfFloor = (int)Math.Ceiling(height / TypicalHeight);
        }
        private void Calculate()
        {
            try
            {
                string path = string.Empty;

                if (File.Exists(@"C:\Users\Default\Desktop\testGh.gh"))
                    path = @"C:\Users\Default\Desktop\testGh.gh";
                else if (File.Exists(@"C: \Users\Admin\Desktop\testGh.gh"))
                    path = @"C:\Users\Admin\Desktop\testGh.gh";
                else
                    throw new Exception("File not found in C:\\Users\\Default\\Desktop\\testGh.gh");

                var io = new GH_DocumentIO();
                io.Open(path);

                GrasshopperDocument = io.Document;

                var activeObjects = GrasshopperDocument.ActiveObjects();
                Output = string.Empty;
                foreach (var obj in activeObjects)
                {
                    if (obj is Grasshopper.Kernel.Special.GH_Panel)
                    {
                        var panel = obj as Grasshopper.Kernel.Special.GH_Panel;
                        if (panel.NickName == "FP")
                        {
                            panel.SetUserText(Footprint.ToString());
                        }
                        else if (panel.NickName == "NF")
                        {
                            panel.SetUserText(NoOfFloor.ToString());

                        }
                        else if (panel.NickName == "LO")
                        {
                            if (Location == "Africa")
                                panel.SetUserText("0");
                            else if (Location == "Asia-Pacific")
                                panel.SetUserText("1");
                            else if (Location == "Europe")
                                panel.SetUserText("2");
                            else if (Location == "Middle East")
                                panel.SetUserText("3");
                            else if (Location == "North America")
                                panel.SetUserText("4");
                            else
                                panel.SetUserText("5");
                        }
                        else if (panel.NickName == "BT")
                        {
                            if (IsCommercial)
                                panel.SetUserText("0");
                            else
                                panel.SetUserText("1");
                        }
                        else if (panel.NickName == "TA")
                        {
                            panel.SetUserText(TotalArea.ToString());

                        }
                        else if (panel.NickName == "OA")
                        {
                            panel.SetUserText(OptionA.ToString());

                        }
                        else if (panel.NickName == "OB")
                        {
                            panel.SetUserText(OptionB.ToString());

                        }
                        else if (panel.NickName == "TS")
                        {
                            if (IsTimber)
                                panel.SetUserText("2");
                            else if (IsConcreteTimber)
                                panel.SetUserText("3");
                            else if (IsConcrete)
                                panel.SetUserText("0");
                            else /*if (IsSteelConcrete)*/
                                panel.SetUserText("1");
                            //else
                            //    panel.SetUserText("Steel");
                        }
                        else if (panel.NickName == "TC")
                        {
                            panel.SetUserText(TargetCo2.ToString());
                        }
                        else if (panel.NickName == "OT")
                        {
                            panel.ExpireSolution(true);
                            panel.CollectData();
                            foreach (var data in panel.VolatileData.AllData(true))
                                Output += data.ToString();

                            if (OptionA)
                            {
                                IsConWoodHyb = false;
                                IsWood = false;
                                IsSteConHyb = false;
                                IsRc = false;
                                Output += " tCo2e";
                            }
                            else if (OptionB)
                            {
                                if (string.Equals(Output, "Wood-Hybrid"))
                                {
                                    IsConWoodHyb = true;
                                    IsWood = false;
                                    IsSteConHyb = false;
                                    IsRc = false;
                                }
                                else if (string.Equals(Output, "Wood"))
                                {
                                    IsConWoodHyb = false;
                                    IsWood = true;
                                    IsSteConHyb = false;
                                    IsRc = false;
                                }
                                else if (string.Equals(Output, "Steel-Concrete"))
                                {
                                    IsConWoodHyb = false;
                                    IsWood = false;
                                    IsSteConHyb = true;
                                    IsRc = false;
                                }
                                else
                                {
                                    IsConWoodHyb = false;
                                    IsWood = false;
                                    IsSteConHyb = false;
                                    IsRc = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Output = e.Message;
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

