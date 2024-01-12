using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MongoDB.Driver;
using MongoDB.Bson;

using Microsoft.Win32;
using System.IO;
using System.Reflection;
using static Exercise1_GameObjectEditor.Spaceship;
using static Exercise1_GameObjectEditor.Officer;

namespace Exercise1_GameObjectEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Spaceship> spaceships = new List<Spaceship>();
        private List<Officer> officers = new List<Officer>();
        private List<PlanetarySystem> planetarySystems = new List<PlanetarySystem>();
        private List<Mission> missions = new List<Mission>();
        MongoClient dbClient = new MongoClient("mongodb+srv://arialian:h5mL9Nr5CbPoezda@cluster0.nc60pq6.mongodb.net/");

        public MainWindow()
        {
            InitializeComponent();
            RefreshValue();
        }

        private void RefreshValue()
        {
            RefreshSpaceships();
            RefreshOfficers();
            RefreshPlanetarySystems();
            RefreshMissions();
        } 

#region Menu
        private async void MenuMongoLoad_Click(object sender, RoutedEventArgs e)
        {
            var database = dbClient.GetDatabase("PROG56693");
            List<Spaceship> spaceshipsCollection =
                await database.GetCollection<Spaceship>("Spaceships").Find(_ => true).As<Spaceship>().ToListAsync();
            spaceships = spaceshipsCollection;

            List<Officer> officersCollection =
                await database.GetCollection<Officer>("Officers").Find(_ => true).As<Officer>().ToListAsync();
            officers = officersCollection;

            List<PlanetarySystem> planetarySystemsCollection =
                await database.GetCollection<PlanetarySystem>("PlanetarySystems").Find(_ => true).As<PlanetarySystem>().ToListAsync();
            planetarySystems = planetarySystemsCollection;

            List<Mission> missionsCollection =
                await database.GetCollection<Mission>("Missions").Find(_ => true).As<Mission>().ToListAsync();
            missions = missionsCollection;

            RefreshValue();

            MessageBox.Show("MongoDB download complete!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuMongoSave_Click(object sender, RoutedEventArgs e)
        {
            var database = dbClient.GetDatabase("PROG56693");

            var spaceshipsCollection = database.GetCollection<Spaceship>("Spaceships");
            if (spaceships.Any())
            {
                spaceshipsCollection.DeleteMany(new BsonDocument());
                spaceshipsCollection.InsertMany(spaceships);
            }
            else
            {
                MessageBox.Show("None spaceship uploaded!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
                
            var officersCollection = database.GetCollection<Officer>("Officers");
            if (officers.Any())
            {
                officersCollection.DeleteMany(new BsonDocument());
                officersCollection.InsertMany(officers);
            }
            else
            {
                MessageBox.Show("None officer uploaded!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            var planetarySystemsCollection = database.GetCollection<PlanetarySystem>("PlanetarySystems");
            if (planetarySystems.Any())
            {
                planetarySystemsCollection.DeleteMany(new BsonDocument());
                planetarySystemsCollection.InsertMany(planetarySystems);
            }
            else
            {
                MessageBox.Show("None planetary system uploaded!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            var missionsCollection = database.GetCollection<Mission>("Missions");
            if (missions.Any())
            {
                missionsCollection.DeleteMany(new BsonDocument());
                missionsCollection.InsertMany(missions);
            }
            else
            {
                MessageBox.Show("None mission uploaded!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            MessageBox.Show("MongoDB upload complete!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }

#endregion Menu

#region Spaceships
        private void RefreshSpaceships()
        {
            dgSpaceShips.ItemsSource = null;
            dgSpaceShips.ItemsSource = spaceships;

            var shipNames = from n in spaceships
                            select n.ShipName;
            cmbShips.ItemsSource = shipNames;
            cmbShips.SelectedItem = null;

            var shipClass = Enum.GetValues(typeof(Spaceship.ShipClassType)).Cast<Spaceship.ShipClassType>();
            cmbShipClass.ItemsSource = shipClass;
            cmbShipClass.SelectedItem = null;

            slShipStrength.Value = 1;
            slWarpRange.Value = 1;
            slWarpSpeed.Value = 1;
            lbShipStrength.Content = "Ship Strength: ";
            lbWarpRange.Content = "Warp Range: ";
            lbWarpSpeed.Content = "Warp Speed: ";
            tbShipName.Text = "";
            tbSpecialAbility.Text = "";
        }

        private void slShipStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbShipStrength.Content = "Ship Strength: " + Convert.ToInt32(slShipStrength.Value);
        }

        private void slWarpRange_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbWarpRange.Content = "Warp Range: " + Convert.ToInt32(slWarpRange.Value);
        }

        private void slWarpSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbWarpSpeed.Content = "Warp Speed: " + Convert.ToDecimal(slWarpSpeed.Value);
        }

        private void btnShipAdd_Click(object sender, RoutedEventArgs e)
        {
            if ((tbShipName.Text == "") || (tbSpecialAbility.Text ==""))
            {
                MessageBox.Show("No textbox can be empty!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (cmbShipClass.SelectedItem == null)
                {
                    MessageBox.Show("Ship class not selected!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    ShipClassType shipClass = (ShipClassType)cmbShipClass.SelectedItem;

                    Spaceship ss = new Spaceship(tbShipName.Text, shipClass, tbSpecialAbility.Text,
                                            Convert.ToInt32(slShipStrength.Value), Convert.ToInt32(slWarpRange.Value), Convert.ToDecimal(slWarpSpeed.Value));
                    spaceships.Add(ss);
                    MessageBox.Show("Spaceship added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshSpaceships();
                }
            }
        }

        private void btnShipUpdate_Click(object sender, RoutedEventArgs e)
        {
            if ((tbShipName.Text == "") || (tbSpecialAbility.Text == ""))
            {
                MessageBox.Show("No textbox can be empty!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (cmbShipClass.SelectedItem == null)
                {
                    MessageBox.Show("Ship class not selected!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (tbShipName.Text != cmbShips.Text)
                    {
                        MessageBox.Show("No spaceship found!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Spaceship ss = new Spaceship(tbShipName.Text, (ShipClassType)cmbShipClass.SelectedItem, tbSpecialAbility.Text,
                                                Convert.ToInt32(slShipStrength.Value), Convert.ToInt32(slWarpRange.Value), Convert.ToDecimal(slWarpSpeed.Value));
                        spaceships[cmbShips.SelectedIndex] = ss;
                        MessageBox.Show("Spaceship update completed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        RefreshSpaceships();
                    }
                }
            }
        }

        private void btnShipDelete_Click(object sender, RoutedEventArgs e)
        {
            spaceships.RemoveAt(cmbShips.SelectedIndex);
            MessageBox.Show("Spaceship removed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            RefreshSpaceships();
        }

        private void cmbShips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int i = cmbShips.SelectedIndex;
                Spaceship ss = spaceships[i];

                tbShipName.Text = ss.ShipName;
                cmbShipClass.SelectedItem = ss.ShipClass;
                tbSpecialAbility.Text = ss.SpecialAbility;
                slShipStrength.Value = ss.ShipStrength;
                slWarpRange.Value = ss.WarpRange;
                slWarpSpeed.Value = Convert.ToDouble(ss.WarpSpeed);
            }
            catch (Exception ex)
            {

            }
        }

#endregion Spaceships

#region Officers
        private void RefreshOfficers()
        {
            dgOfficers.ItemsSource = null;
            dgOfficers.ItemsSource = officers;

            var officerNames = from n in officers
                               select n.OfficerName;
            cmbOfficers.ItemsSource = officerNames;
            cmbOfficers.SelectedItem = null;

            var shipSpecialty = Enum.GetValues(typeof(Spaceship.ShipClassType)).Cast<Spaceship.ShipClassType>();
            cmbShipSpecialty.ItemsSource = shipSpecialty;
            cmbShipSpecialty.SelectedItem = null;

            var officerRace = from r in planetarySystems
                              select r.IndigenousRace;
            cmbOfficerRace.ItemsSource = officerRace;
            cmbOfficerRace.SelectedItem = null;

            var homePlanetSystem = from n in planetarySystems
                                   select n.PlanetarySystemName;
            cmbHomePlanetSystem.ItemsSource = homePlanetSystem;
            cmbHomePlanetSystem.SelectedItem = null; 
            
            slAttackStrength.Value = 1;
            slDefenceStrength.Value = 1;
            slHealthStrength.Value = 1;
            pbOverallStrength.Value = 0;
            lbAttackStrength.Content = "Attack Strength: ";
            lbDefenceStrength.Content = "Defence Strength: ";
            lbHealthStrength.Content = "Health Strength: ";
            lbOverallStrength.Content = "Overall Strength: ";
            tbOfficerName.Text = "";
        }

        private void slAttackStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbAttackStrength.Content = "Attack Strength: " + Convert.ToInt32(slAttackStrength.Value);
            UpdateOverallStrength();
        }

        private void slDefenceStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbDefenceStrength.Content = "Defence Strength: " + Convert.ToInt32(slDefenceStrength.Value);
            UpdateOverallStrength();
        }

        private void slHealthStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbHealthStrength.Content = "Health Strength: " + Convert.ToInt32(slHealthStrength.Value);
            UpdateOverallStrength();
        }

        private void pbOverallStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbOverallStrength.Content = "Overall Strength: " + Convert.ToInt32(pbOverallStrength.Value);
        }

        private void UpdateOverallStrength()
        {
            if ((slAttackStrength.Value != 1) && (slDefenceStrength.Value != 1) && (slHealthStrength.Value != 1))
            {
                pbOverallStrength.Value = (slAttackStrength.Value + slDefenceStrength.Value + slHealthStrength.Value) / 3;
            }
            else
            {
                if (pbOverallStrength != null)
                {
                    pbOverallStrength.Value = (slAttackStrength.Value + slDefenceStrength.Value + slHealthStrength.Value) / 3;
                }
            }
        }

        private void btnOfficerAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbOfficerName.Text == "")
            {
                MessageBox.Show("No textbox can be empty!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if ((cmbOfficerRace.SelectedItem == null) || (cmbShipSpecialty.SelectedItem == null) || (cmbHomePlanetSystem.SelectedItem == null))
                {
                    MessageBox.Show("No combo box can be empty! Add the Planet System first if there is no data can be selected.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    Officer ofc = new Officer(tbOfficerName.Text, Convert.ToString(cmbOfficerRace.SelectedItem),
                                        Convert.ToInt32(slAttackStrength.Value), Convert.ToInt32(slDefenceStrength.Value),
                                        Convert.ToInt32(slHealthStrength.Value), Convert.ToInt32(pbOverallStrength.Value),
                                        (ShipClassType)cmbShipSpecialty.SelectedItem, Convert.ToString(cmbHomePlanetSystem.SelectedItem));
                    officers.Add(ofc);
                    MessageBox.Show("Officer added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshOfficers();
                }
            }
        }

        private void btnOfficerUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (tbOfficerName.Text == "")
            {
                MessageBox.Show("No textbox can be empty!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if ((cmbOfficerRace.SelectedItem == null) || (cmbShipSpecialty.SelectedItem == null) || (cmbHomePlanetSystem.SelectedItem == null))
                {
                    MessageBox.Show("No combo box can be empty! Add the Planet System first if there is no data can be selected.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (tbOfficerName.Text != cmbOfficers.Text)
                    {
                        MessageBox.Show("No officer found!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Officer ofc = new Officer(tbOfficerName.Text, Convert.ToString(cmbOfficerRace.SelectedItem),
                                            Convert.ToInt32(slAttackStrength.Value), Convert.ToInt32(slDefenceStrength.Value),
                                            Convert.ToInt32(slHealthStrength.Value), Convert.ToInt32(pbOverallStrength.Value),
                                            (ShipClassType)cmbShipSpecialty.SelectedItem, Convert.ToString(cmbHomePlanetSystem.SelectedItem));
                        officers[cmbOfficers.SelectedIndex] = ofc;
                        MessageBox.Show("Officer update completed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        RefreshOfficers();
                    }
                }
            }
        }

        private void btnOfficerDelete_Click(object sender, RoutedEventArgs e)
        {
            officers.RemoveAt(cmbOfficers.SelectedIndex);
            MessageBox.Show("Officer removed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            RefreshOfficers();
        }

        private void cmbOfficers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int i = cmbOfficers.SelectedIndex;
                Officer ofc = officers[i];

                tbOfficerName.Text = ofc.OfficerName;
                cmbOfficerRace.SelectedItem = ofc.OfficerRace;
                slAttackStrength.Value = ofc.AttackStrength;
                slDefenceStrength.Value = ofc.DefenceStrength;
                slHealthStrength.Value = ofc.HealthStrength;
                pbOverallStrength.Value = ofc.OverallStrength;
                cmbShipSpecialty.SelectedItem = ofc.ShipSpecialty;
                cmbHomePlanetSystem.SelectedItem = ofc.HomePlanetSystem;
            }
            catch (Exception ex)
            {

            }
        }

#endregion Officers

#region PlanetarySystems
        private void RefreshPlanetarySystems()
        {
            dgPlanetarySystems.ItemsSource = null;
            dgPlanetarySystems.ItemsSource = planetarySystems;

            var planetarySystemNames = from n in planetarySystems
                                       select n.PlanetarySystemName;
            cmbPlanetarySystems.ItemsSource = planetarySystemNames;
            cmbPlanetarySystems.SelectedItem = null;

            slNumberofPlanets.Value = 1;
            lbNumberofPlanets.Content = "Number of Planets: ";
            tbPlanetarySystemName.Text = "";
            tbIndigenousRace.Text = "";
        }

        private void slNumberofPlanets_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbNumberofPlanets.Content = "Number of Planets: " + Convert.ToInt32(slNumberofPlanets.Value);
        }

        private void btnPlanetarySystemAdd_Click(object sender, RoutedEventArgs e)
        {
            if ((tbPlanetarySystemName.Text == "") || (tbIndigenousRace.Text == ""))
            {
                MessageBox.Show("No textbox can be empty!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                PlanetarySystem ps = new PlanetarySystem(tbPlanetarySystemName.Text, tbIndigenousRace.Text,
                                            Convert.ToInt32(slNumberofPlanets.Value));
                planetarySystems.Add(ps);
                MessageBox.Show("Planetary system added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshValue();
            }
        }

        private void btnPlanetarySystemUpdate_Click(object sender, RoutedEventArgs e)
        {
            if ((tbPlanetarySystemName.Text == "") || (tbIndigenousRace.Text == ""))
            {
                MessageBox.Show("No textbox can be empty!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (tbPlanetarySystemName.Text != cmbPlanetarySystems.Text)
                {
                    MessageBox.Show("No planetary system found!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    PlanetarySystem ps = new PlanetarySystem(tbPlanetarySystemName.Text, tbIndigenousRace.Text,
                                            Convert.ToInt32(slNumberofPlanets.Value));
                    planetarySystems[cmbPlanetarySystems.SelectedIndex] = ps;

                    //Check the Officer Race with Planetary System's IndigenousRace
                    if (officers.Any())
                    {
                        foreach (Officer ofc in officers)
                        {
                            if (!planetarySystems.Any(ps => ps.IndigenousRace == ofc.OfficerRace))
                            {
                                MessageBoxResult result = MessageBox.Show("The officer " + ofc.OfficerName + " is the race " + ofc.OfficerRace + ".\n" +
                                                                          "Do you want to update " + ofc.OfficerName + " 's race to " + tbIndigenousRace.Text + "?" + "\n" +
                                                                          "If you chose No, " + ofc.OfficerName + " 's race will be empty!", 
                                                                          "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                if (result == MessageBoxResult.Yes)
                                {
                                    ofc.OfficerRace = tbIndigenousRace.Text;
                                    MessageBox.Show("The officer " + ofc.OfficerName + "'s race is updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    if (result == MessageBoxResult.No)
                                    {
                                        ofc.OfficerRace = "";
                                    }
                                }
                            }
                        }
                    }

                    MessageBox.Show("Planetary system update completed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshValue();
                }
            }
        }

        private void btnPlanetarySystemDelete_Click(object sender, RoutedEventArgs e)
        {
            planetarySystems.RemoveAt(cmbPlanetarySystems.SelectedIndex);

            //Check the Officer's Home Planet System with Planetary System Name
            if (officers.Any())
            {
                foreach (Officer ofc in officers)
                {
                    if (!planetarySystems.Any(ps => ps.PlanetarySystemName == ofc.HomePlanetSystem))
                    {
                        MessageBoxResult result = MessageBox.Show("The officer " + ofc.OfficerName + " is from the deleted planetary system " + ofc.HomePlanetSystem + ".\n" +
                                                                  "Do you want to delete the officer " + ofc.OfficerName + "?" + "\n" + 
                                                                  "If you chose No, " + ofc.OfficerName + " 's home planet system will be empty!",
                                                                  "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Yes)
                        {
                            officers.Remove(ofc);
                            MessageBox.Show("The officer " + ofc.OfficerName + "is deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if (result == MessageBoxResult.No)
                            {
                                ofc.HomePlanetSystem = "";
                            }
                        }
                    }
                }
            }

            //Check the Officer Race with Planetary System's IndigenousRace
            if (officers.Any())
            {
                foreach (Officer ofc in officers)
                {
                    if (!planetarySystems.Any(ps => ps.IndigenousRace == ofc.OfficerRace))
                    {
                        MessageBoxResult result = MessageBox.Show("The officer " + ofc.OfficerName + " is the race " + ofc.OfficerRace + ".\n" +
                                                                  "Do you want to delete the officer " + ofc.OfficerName + "?" + "\n" +
                                                                  "If you chose No, " + ofc.OfficerName + " 's race will be empty!",
                                                                  "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Yes)
                        {
                            officers.Remove(ofc);
                            MessageBox.Show("The officer " + ofc.OfficerName + "is deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if (result == MessageBoxResult.No)
                            {
                                ofc.OfficerRace = "";
                            }
                        }
                    }
                }

            }

            //Check the Mission's location with Planetary System Name
            if (missions.Any())
            {
                foreach (Mission m in missions)
                {
                    if (!planetarySystems.Any(ps => ps.PlanetarySystemName == m.Location))
                    {
                        MessageBoxResult result = MessageBox.Show("The mission " + m.MissionName + " is in the deleted planetary system " + m.Location + ".\n" +
                                                                  "Do you want to delete the mission " + m.MissionName + "?" + "\n" +
                                                                  "If you chose No, " + m.MissionName + " 's location will be empty!",
                                                                  "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Yes)
                        {
                            missions.Remove(m);
                            MessageBox.Show("The mission " + m.MissionName + "is deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            if (result == MessageBoxResult.No)
                            {
                                m.Location = "";
                            }
                        }
                    }
                }
            }

            MessageBox.Show("Planetary system removed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            RefreshValue();
        }

        private void cmbPlanetarySystems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int i = cmbPlanetarySystems.SelectedIndex;
                PlanetarySystem ps = planetarySystems[i];

                tbPlanetarySystemName.Text = ps.PlanetarySystemName;
                tbIndigenousRace.Text = ps.IndigenousRace;
                slNumberofPlanets.Value = ps.NumberOfPlanets;
            }
            catch (Exception ex)
            {

            }
        }

#endregion PlanetarySystems

#region Missions
        private void RefreshMissions()
        {
            dgMissions.ItemsSource = null;
            dgMissions.ItemsSource = missions;

            var missionNames = from n in missions
                               select n.MissionName;
            cmbMissions.SelectedItem = null;
            cmbMissions.ItemsSource = missionNames;

            var location = from n in planetarySystems
                              select n.PlanetarySystemName;
            cmbLocation.ItemsSource = location;
            cmbLocation.SelectedItem = null;

            tbMissionName.Text = "";
            tbRewards.Text = "";
            tbDescription.Text = "";
        }

        private void btnMissionAdd_Click(object sender, RoutedEventArgs e)
        {
            if ((tbMissionName.Text == "") || (tbRewards.Text == "") || (tbDescription.Text == ""))
            {
                MessageBox.Show("No textbox can be empty!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (cmbLocation.SelectedItem == null)
                {
                    MessageBox.Show("Location not selected! Add the Planet System first if there is no data can be selected.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    Mission m = new Mission(tbMissionName.Text, tbRewards.Text, tbDescription.Text, Convert.ToString(cmbLocation.SelectedItem));
                    missions.Add(m);
                    MessageBox.Show("Mission added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshMissions();
                }
            }
        }

        private void btnMissionUpdate_Click(object sender, RoutedEventArgs e)
        {
            if ((tbMissionName.Text == "") || (tbRewards.Text == "") || (tbDescription.Text == ""))
            {
                MessageBox.Show("No textbox can be empty!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (cmbLocation.SelectedItem == null)
                {
                    MessageBox.Show("Location not selected!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (tbMissionName.Text != cmbMissions.Text)
                    {
                        MessageBox.Show("No mission found!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Mission m = new Mission(tbMissionName.Text, tbRewards.Text, tbDescription.Text, Convert.ToString(cmbLocation.SelectedItem));
                        missions[cmbMissions.SelectedIndex] = m;
                        MessageBox.Show("Mission update completed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        RefreshMissions();
                    }
                }
            }
        }

        private void btnMissionDelete_Click(object sender, RoutedEventArgs e)
        {
            missions.RemoveAt(cmbMissions.SelectedIndex);
            MessageBox.Show("Mission removed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            RefreshMissions();
        }

        private void cmbMissions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int i = cmbMissions.SelectedIndex;
                Mission m = missions[i];

                tbMissionName.Text = m.MissionName;
                tbRewards.Text = m.Rewards;
                tbDescription.Text = m.Description;
                cmbLocation.SelectedItem = m.Location;
            }
            catch (Exception ex)
            {

            }
        }

#endregion Missions
       
    }
}
