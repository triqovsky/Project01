using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using WSOS_v0._1.Properties;
using System.IO;

namespace WSOS_v0._1
{
    public partial class AdminForm : Form,ISzukanie
    {
        MySqlCommand dodajPlanKom;
        string dodajPlanSql;
        public MySqlDataReader czytnik;



        public AdminForm()
        {
            InitializeComponent();
            
        }
  
        void DodajPlan(string sciezkaPlanu)
        {
            try
            {
                
                //Strumieniowanie przesyłania pliku
                FileStream planStream = new FileStream(sciezkaPlanu, FileMode.Open, FileAccess.Read);

                //Utworzenie tablicy bajtów potrzebnej do przesłania planu
                byte[] daneplan = new byte[planStream.Length];
                planStream.Read(daneplan, 0, (int)planStream.Length);
                planStream.Close();

                //"Przetłumaczenie" tablicy bajtów na string w syst. szesnastkowym
                string hexplan = BitConverter.ToString(daneplan);
                hexplan = hexplan.Replace("-", "");

                //Sprawdzanie czy polaczenie jest otwarte
                if (FormularzLogIn.pol.State == ConnectionState.Closed)
                {
                    FormularzLogIn.pol.Open();
                }
                //Wykonanie komendy w SQL i odczytanie jej wyniku
               
                dodajPlanSql = String.Format("UPDATE Osoby SET Plan=x'{0}' WHERE CONCAT(Nazwisko,' ',Imiona)='{1}'", hexplan,PlanyUsersBox.SelectedItem);
                dodajPlanKom = new MySqlCommand(dodajPlanSql, FormularzLogIn.pol);
                czytnik = dodajPlanKom.ExecuteReader();
                MessageBox.Show("Plan został dodany.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                string blad = string.Format("Błąd dodawania planu.\n{0}", ex.Message);
                MessageBox.Show(blad, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                czytnik.Close();
            }
        }
        public void WyszukajSt()
        {
            MySqlDataReader czytSzukanieStudenta;
            if (FormularzLogIn.pol.State == ConnectionState.Closed)
                FormularzLogIn.pol.Open();
            string sqlSzukanieStudenta = String.Format("SELECT DISTINCT S.NrIndeksu,CONCAT(O.Imiona,' ',O.Nazwisko),O.Email,S.ProgramStudiow,S.Semestr,P.Kierunek,P.Specjalnosc,P.TrybStudiow,P.RodzajStudiow FROM Osoby O INNER JOIN Studenci S ON S.NrIndeksu=O.Identyfikator INNER JOIN Programy P ON P.IdProgramu=S.ProgramStudiow WHERE CONCAT(O.Nazwisko,' ' ,O.Imiona)='{0}'", WyszukajStBox.SelectedItem);
            MySqlCommand SzukanieStudKom = new MySqlCommand(sqlSzukanieStudenta, FormularzLogIn.pol);
            czytSzukanieStudenta = SzukanieStudKom.ExecuteReader();
            listaWyszukiwanychStudentow.Items.Clear();
            if (czytSzukanieStudenta.HasRows)
            {
                while (czytSzukanieStudenta.Read())
                {
                    listaWyszukiwanychStudentow.Items.Add(string.Format("{0}    {1}       {2}       {3}       semestr  {4}       {5}       {6}      studia {7} {8}", czytSzukanieStudenta[0].ToString(), czytSzukanieStudenta[1].ToString(), czytSzukanieStudenta[2].ToString(), czytSzukanieStudenta[3].ToString(), czytSzukanieStudenta[4].ToString(), czytSzukanieStudenta[5].ToString(), czytSzukanieStudenta[6].ToString(), czytSzukanieStudenta[7].ToString(),czytSzukanieStudenta[8].ToString()));
                }

            }
            czytSzukanieStudenta.Close();
        }
        public void WyszukajPrac()
        {
            MySqlDataReader czytSzukaniePracownika;
            if (FormularzLogIn.pol.State == ConnectionState.Closed)
                FormularzLogIn.pol.Open();
            string sqlSzukaniePracownika = String.Format("SELECT DISTINCT O.Identyfikator,P.Tytul,CONCAT(O.Imiona,' ',O.Nazwisko),O.Email,P.WWW,P.NrTel,P.Pokoj,J.NazwaJednostki,S.NazwaStan FROM Osoby O INNER JOIN Pracownicy P ON P.IdPrac=O.Identyfikator INNER JOIN Jednostki J ON J.IdJednostki=P.Jednostka INNER JOIN Stanowiska S ON S.IdStan=P.Stanowisko WHERE CONCAT(O.Nazwisko,' ' ,O.Imiona)='{0}'", WyszukajPracBox.SelectedItem);
            MySqlCommand SzukaniePracKom = new MySqlCommand(sqlSzukaniePracownika, FormularzLogIn.pol);
            czytSzukaniePracownika = SzukaniePracKom.ExecuteReader();
            listaWyszukiwanychPracownikow.Items.Clear();
            if (czytSzukaniePracownika.HasRows)
            {
                while (czytSzukaniePracownika.Read())
                {
                    listaWyszukiwanychPracownikow.Items.Add(string.Format("{0} {1}     {2}     {3}     {4}     {5}     {6}     {7}", czytSzukaniePracownika[0].ToString(), czytSzukaniePracownika[1].ToString(), czytSzukaniePracownika[2].ToString(), czytSzukaniePracownika[3].ToString(), czytSzukaniePracownika[4].ToString(), czytSzukaniePracownika[5].ToString(), czytSzukaniePracownika[6].ToString(),czytSzukaniePracownika[7].ToString()));
                }

            }
            czytSzukaniePracownika.Close();
        }
  
        private void ZamknijBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DodajPlanBTN_Click(object sender, EventArgs e)
        {
            //Obługuje okno dialogowe wyboru pliku do przesłania
            OpenFileDialog wyborPlanu = new OpenFileDialog();
            wyborPlanu.Filter = "Pliki graficzne|*.png;*.jpg;*.jpeg;*.bmp";
            if (wyborPlanu.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                DodajPlan(wyborPlanu.FileName);

        }
        
        private void DodajPracBTN_Click(object sender, EventArgs e)
        {
            string dodajPracSql2 = String.Format("INSERT INTO osoby(Identyfikator,Nazwisko,Imiona,Email,Wydzial,Rola,Login,Haslo,Plan)VALUES({0},'{1}','{2}','{3}',0,'{4}','{5}','pracownik','0')",IdPBox.Text, NazwiskoPBox.Text, ImionaPBox.Text, EmailPBox.Text, RolaBox.Text, LoginPBox.Text);
            MySqlCommand DodajPracKom2 = new MySqlCommand(dodajPracSql2, FormularzLogIn.pol);
            DodajPracKom2.ExecuteNonQuery();

            string jakaKatedraSql = String.Format("SELECT IdJednostki FROM Jednostki WHERE NazwaJednostki='{0}'",KatedraBox.SelectedItem);
            MySqlCommand jakaKatedraKom = new MySqlCommand(jakaKatedraSql,FormularzLogIn.pol);
            string jakaKatedra = Convert.ToString(jakaKatedraKom.ExecuteScalar());

            string jakieStanSql = string.Format("SELECT IdStan FROM Stanowiska WHERE NazwaStan='{0}'",StanowiskoBox.SelectedItem);
            MySqlCommand jakieStanKom = new MySqlCommand(jakieStanSql,FormularzLogIn.pol);
            string jakieStan = Convert.ToString(jakieStanKom.ExecuteScalar());

            string dodajPracSql1 = String.Format("INSERT INTO pracownicy(IdPrac,Tytul,Stanowisko,WWW,NrTel,Pokoj,Jednostka)VALUES({0},'{1}',{2},'{3}','{4}','{5}',{6})", IdPBox.Text,TytulBox.Text, jakieStan, WWWBox.Text, TelBox.Text, PokojBox.Text, jakaKatedra);
            MySqlCommand DodajPracKom1 = new MySqlCommand(dodajPracSql1, FormularzLogIn.pol);
            DodajPracKom1.ExecuteNonQuery();
            IdPBox.Clear();
            NazwiskoPBox.Clear();
            ImionaPBox.Clear();
            EmailPBox.Clear();
            LoginPBox.Clear();
            WWWBox.Clear();
            TelBox.Clear();
            PokojBox.Clear();
            MessageBox.Show("Dodano nowego pracownika. Zgłoś administracji, aby założono mu konto w systemie!", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        void PobierzDane()
        {
            //Ta część odpowiada za pobranie z bazy programów studiów i umieszczenie ich w ComboBoxie
            string pobierzPrSql = String.Format("SELECT IdProgramu FROM Programy");
            MySqlCommand pobierzPrKom = new MySqlCommand(pobierzPrSql, FormularzLogIn.pol);
            MySqlDataReader czytPr = pobierzPrKom.ExecuteReader();
            if (czytPr.HasRows)
            {
                while (czytPr.Read())
                {
                    ProgramSBox.Items.Add(czytPr[0].ToString());
                    ProgramS2Box.Items.Add(czytPr[0].ToString());
                    WyszProgBox.Items.Add(czytPr[0].ToString());
                    WyszPrzedBox2.Items.Add(czytPr[0].ToString());

                }
                czytPr.Close();
            }
            //Ta część odpowiada za pobranie z bazy nazw stanowisk i umieszczenie ich w ComboBoxie
            string stanowiskaSql = String.Format("SELECT NazwaStan FROM Stanowiska");
            MySqlCommand stanowiskaKom = new MySqlCommand(stanowiskaSql, FormularzLogIn.pol);
            MySqlDataReader czytStan = stanowiskaKom.ExecuteReader();
            if (czytStan.HasRows)
            {

                while (czytStan.Read())
                {
                    StanowiskoBox.Items.Add(czytStan[0].ToString());

                }
                czytStan.Close();
            }
            //Ta część odpowiada za pobranie z bazy katedr i umieszczenie ich w ComboBoxie
            string katedrySql = String.Format("SELECT NazwaJednostki FROM Jednostki");
            MySqlCommand katedryKom = new MySqlCommand(katedrySql, FormularzLogIn.pol);
            MySqlDataReader czytKatedry = katedryKom.ExecuteReader();
            if (czytKatedry.HasRows)
            {
                while (czytKatedry.Read())
                {
                    KatedraBox.Items.Add(czytKatedry[0].ToString());
                }
                czytKatedry.Close();
            }
            //Ta część odpowiada za pobranie z bazy użytkowników i umieszczenie ich w ComboBoxie
            string wyswietlUsers = String.Format("Select CONCAT(Nazwisko,' ',Imiona) FROM Osoby WHERE Rola='student' OR Rola='pracownik dydaktyczny'");
            MySqlCommand pokazUsersKom = new MySqlCommand(wyswietlUsers, FormularzLogIn.pol);
            MySqlDataReader czytnikUsers = pokazUsersKom.ExecuteReader();
            if (czytnikUsers.HasRows)
            {
                while (czytnikUsers.Read())
                {
                    PlanyUsersBox.Items.Add(czytnikUsers[0].ToString());

                }

                czytnikUsers.Close();

            }
            //Pobranie informacji o studentach do ComboBoxa
            string pobierzStSql = String.Format("SELECT DISTINCT CONCAT(Nazwisko,' ',Imiona) FROM Osoby Os INNER JOIN Studenci S ON S.NrIndeksu=Os.Identyfikator");
            MySqlCommand pobierzStKom = new MySqlCommand(pobierzStSql, FormularzLogIn.pol);
            MySqlDataReader czytSt = pobierzStKom.ExecuteReader();
            if (czytSt.HasRows)
            {
                while (czytSt.Read())
                {
                    WyszukajStBox.Items.Add(czytSt[0].ToString());

                }

            }
            czytSt.Close();

            //Pobranie informacji o pracownikach do ComboBoxa
            string pobierzPracsql = String.Format("SELECT CONCAT(Nazwisko,' ',Imiona)FROM Osoby WHERE Rola='pracownik administracyjny'OR Rola='pracownik dydaktyczny'");
            MySqlCommand pobierzPracKom = new MySqlCommand(pobierzPracsql, FormularzLogIn.pol);
            MySqlDataReader czytPrac = pobierzPracKom.ExecuteReader();
            if (czytPrac.HasRows)
            {
                while (czytPrac.Read())
                {
                    WyszukajPracBox.Items.Add(czytPrac[0].ToString());
                }
            }
            czytPrac.Close();

            //Pobieranie informacji o przedmiotach do CombBoxa
            string pobierzPrzSql = String.Format("SELECT NazwaPrzedmiotu from Przedmioty");
            MySqlCommand pobierzPrzKom = new MySqlCommand(pobierzPrzSql, FormularzLogIn.pol);
            MySqlDataReader czytPrz = pobierzPrzKom.ExecuteReader();
            if (czytPrz.HasRows)
            {
                while (czytPrz.Read())
                {
                    WyszPrzedBox.Items.Add(czytPrz[0].ToString());
                }

            }
            czytPrz.Close();
            
        }
        private void PobierzDaneSBTN_Click(object sender, EventArgs e)
        {
            PobierzDane();
        }

        private void DodajStdBTN_Click(object sender, EventArgs e)
        {
            string dodajStudSql = String.Format("INSERT INTO Osoby (Identyfikator,Nazwisko,Imiona,Email,Wydzial,Rola,Login,Haslo,Plan)VALUES({0},'{1}','{2}','{3}',0,'student','s{4}','student','0')",IDSBox.Text,NazwiskoSBox.Text,ImionaSBox.Text,EmailSBox.Text,IDSBox.Text);
            string dodajStudSql2 = String.Format("INSERT INTO Studenci(NrIndeksu,ProgramStudiow,Semestr,GrKon,GrLab,GrWyk)VALUES({0},'{1}',{2},{3},{4},{5})",IDSBox.Text,ProgramSBox.SelectedItem, SemestrSBox.SelectedItem,GrKonBox.Text,GrLabBox.Text,GrWykBox.Text);
            MySqlCommand dodajStudKom1 = new MySqlCommand(dodajStudSql, FormularzLogIn.pol);
            MySqlCommand dodajStudKom2 = new MySqlCommand(dodajStudSql2, FormularzLogIn.pol);
            dodajStudKom1.ExecuteNonQuery();
            dodajStudKom2.ExecuteNonQuery();
            IDSBox.Clear();
            NazwiskoSBox.Clear();
            ImionaSBox.Clear();
            EmailSBox.Clear();
            GrKonBox.Clear();
            GrLabBox.Clear();
            GrWykBox.Clear();
            MessageBox.Show("Dodano nowego studenta. Zgłoś administracji, aby założono mu konto w systemie!", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DodajPrzedmiotBTN_Click(object sender, EventArgs e)
        {
            string dodajPrzedSql = String.Format("INSERT INTO Przedmioty(IdPrzedmiotu,NazwaPrzedmiotu)VALUES({0},'{1}')",IdPrzBox.Text,NazwaPrzBox.Text);
            string dodajPrzedSql2 = String.Format("INSERT INTO Detale(ProgramStudiow,Przedmiot,Semestr,ECTS,Zal,WYK,KON,LAB)VALUES('{0}',{1},{2},{3},'{4}',{5},{6},{7})",
                ProgramS2Box.SelectedItem,IdPrzBox.Text,SemestPrzBox.SelectedItem, 
                ECTSBox.Text,ZalBox.SelectedItem, WykPrzBox.Text,KonPrzBox.Text,LabPrzBox.Text);
            MySqlCommand dodajPrzedKom1 = new MySqlCommand(dodajPrzedSql, FormularzLogIn.pol);
            MySqlCommand dodajPrzedKom2 = new MySqlCommand(dodajPrzedSql2, FormularzLogIn.pol);
            dodajPrzedKom1.ExecuteNonQuery();
            dodajPrzedKom2.ExecuteNonQuery();
            IdPrzBox.Clear();
            NazwaPrzBox.Clear();
            KonPrzBox.Clear();
            LabPrzBox.Clear();
            ECTSBox.Clear();
            WykPrzBox.Clear();
            MessageBox.Show("Przedmiot został dodany!", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DodajProgramBTN_Click(object sender, EventArgs e)
        {
            string dodajProgramSql = String.Format("INSERT INTO Programy(IdProgramu,Kierunek,Specjalnosc,RodzajStudiow,TrybStudiow)VALUES('{0}','{1}','{2}','{3}','{4}')",KodProgBox.Text,KierBox.Text,SpecjalBox.Text,RodzajSBox.SelectedItem, TrybSBox.SelectedItem);
            MySqlCommand dodajProKom = new MySqlCommand(dodajProgramSql, FormularzLogIn.pol);
            dodajProKom.ExecuteNonQuery();
            KodProgBox.Clear();
            KierBox.Clear();
            SpecjalBox.Clear();
            MessageBox.Show("Program studiów został dodany!", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void WyszukajPracBTN_Click(object sender, EventArgs e)
        {
            WyszukajPrac();
        }

        private void WyszukajStBTN_Click(object sender, EventArgs e)
        {
            WyszukajSt();
        }
        void EditPrac()
        {
            string jakieStanSql = string.Format("SELECT IdStan FROM Stanowiska WHERE NazwaStan='{0}'", StanowiskoBox.SelectedItem);
            MySqlCommand jakieStanKom = new MySqlCommand(jakieStanSql, FormularzLogIn.pol);
            string jakieStan = Convert.ToString(jakieStanKom.ExecuteScalar());

            string jakaKatedraSql = string.Format("SELECT IdJednostki FROM Jednostki WHERE NazwaJednostki='{0}'", KatedraBox.SelectedItem);
            MySqlCommand jakaKatedraKom = new MySqlCommand(jakaKatedraSql, FormularzLogIn.pol);
            string jakaKatedra = Convert.ToString(jakaKatedraKom.ExecuteScalar());

            //Zapytanie edytujące pracownika w bazie
            string edytujPracSql = String.Format("UPDATE Osoby SET Nazwisko='{0}',Imiona='{1}',Email='{2}',Rola='{3}',Login='{4}' WHERE Identyfikator={5}", NazwiskoPBox.Text, ImionaPBox.Text, EmailPBox.Text, RolaBox.SelectedItem, LoginPBox.Text,IdPBox.Text);
            string edytujaPracSql2 = string.Format("UPDATE Pracownicy SET Tytul='{0}',Stanowisko={1},WWW='{2}',NrTel='{3}',Pokoj='{4}',Jednostka={5} WHERE IdPrac={6}",TytulBox.SelectedItem,jakieStan,WWWBox.Text,TelBox.Text,PokojBox.Text,jakaKatedra,IdPBox.Text);
            MySqlCommand edytujPracKom = new MySqlCommand(edytujPracSql, FormularzLogIn.pol);
            MySqlCommand edytujPracKom2 = new MySqlCommand(edytujaPracSql2,FormularzLogIn.pol);
            edytujPracKom.ExecuteNonQuery();
            edytujPracKom2.ExecuteNonQuery();
            PobierzDane();

        }
        void EditStud()
        {
            string edytujStudSql = String.Format("UPDATE Osoby SET Nazwisko='{0}',Imiona='{1}',Email='{2}' WHERE Identyfikator = {3}",NazwiskoSBox.Text,ImionaSBox.Text,EmailSBox.Text,IDSBox.Text);
            string edytujStudSql2 = String.Format("UPDATE Studenci SET Semestr={0},ProgramStudiow='{1}',GrKon={2},GrLab={3},GrWyk={4} WHERE NrIndeksu={5}",SemestrSBox.SelectedItem,ProgramSBox.SelectedItem,GrWykBox.Text,GrKonBox.Text,GrLabBox.Text,IDSBox.Text);
            MySqlCommand edytujStudKom1 = new MySqlCommand(edytujStudSql,FormularzLogIn.pol);
            MySqlCommand edytujStudKom2 = new MySqlCommand(edytujStudSql2,FormularzLogIn.pol);
            edytujStudKom1.ExecuteNonQuery();
            edytujStudKom2.ExecuteNonQuery();
            PobierzDane();
        }
        void EditPrzed()
        {
            string edytujPrzedSql = String.Format("UPDATE Przedmioty SET NazwaPrzedmiotu='{0}' WHERE IdPrzedmiotu={1}",NazwaPrzBox.Text,IdPrzBox.Text);
            string edytujPrzedSql2 = String.Format("UPDATE Detale SET ECTS={0},Zal='{1}',Semestr={2},WYK={3},KON={4},LAB={5} WHERE Przedmiot={6} AND ProgramStudiow='{7}'",ECTSBox.Text,ZalBox.SelectedItem,SemestPrzBox.SelectedItem,WykPrzBox.Text,KonPrzBox.Text,LabPrzBox.Text,IdPrzBox.Text,ProgramS2Box.SelectedItem);
            MySqlCommand edytujPrzedKom1 = new MySqlCommand(edytujPrzedSql,FormularzLogIn.pol);
            MySqlCommand edytujPrzedKom2 = new MySqlCommand(edytujPrzedSql2, FormularzLogIn.pol);
            edytujPrzedKom1.ExecuteNonQuery();
            edytujPrzedKom2.ExecuteNonQuery();
        }
        void EditProg()
        {
            string edytujProgSql = String.Format("UPDATE Programy SET Kierunek = '{0}',Specjalnosc='{1}',RodzajStudiow='{2}',TrybStudiow='{3}' WHERE IdProgramu='{4}'",KierBox.Text,SpecjalBox.Text,RodzajSBox.SelectedItem,TrybSBox.SelectedItem,KodProgBox.Text);
            MySqlCommand edytujProgKom = new MySqlCommand(edytujProgSql,FormularzLogIn.pol);
            edytujProgKom.ExecuteNonQuery();
        }
        void WyszukajPrzedmiot()
        {
            string wyszPrzedSql = String.Format("SELECT IdPrzedmiotu FROM Przedmioty P INNER JOIN Detale D ON D.Przedmiot=P.IdPrzedmiotu WHERE NazwaPrzedmiotu='{0}' AND ProgramStudiow='{1}'",WyszPrzedBox.SelectedItem,WyszPrzedBox2.SelectedItem);
            MySqlCommand wyszPrzedKom = new MySqlCommand(wyszPrzedSql,FormularzLogIn.pol);
            string jakieIdPrzed = Convert.ToString(wyszPrzedKom.ExecuteScalar());
            string wyszPrzedSql2 = String.Format("SELECT P.NazwaPrzedmiotu,D.ProgramStudiow,D.Semestr,D.ECTS,D.Zal,D.WYK,D.KON,D.LAB FROM Detale D INNER JOIN Przedmioty P ON P.IdPrzedmiotu=D.Przedmiot WHERE D.Przedmiot={0} AND D.ProgramStudiow='{1}'",jakieIdPrzed,WyszPrzedBox2.SelectedItem);
            MySqlCommand wyszPrzedKom2 = new MySqlCommand(wyszPrzedSql2,FormularzLogIn.pol);
            MySqlDataReader czytSzukPrzed = wyszPrzedKom2.ExecuteReader();
            listaWyszPrzed.Items.Clear();
            if(czytSzukPrzed.HasRows)
            {
                while(czytSzukPrzed.Read())
                {
                    listaWyszPrzed.Items.Add(String.Format("{0}          {1}        {2}         {3}       {4}   {5}  {6}  {7}", czytSzukPrzed[0].ToString(),czytSzukPrzed[1].ToString(), czytSzukPrzed[2].ToString(), czytSzukPrzed[3].ToString(), czytSzukPrzed[4].ToString(), czytSzukPrzed[5].ToString(), czytSzukPrzed[6].ToString(),czytSzukPrzed[7].ToString()));
                }

            }
            czytSzukPrzed.Close();
        }
        void WyszukajProgram()
        {
            string wyszProgSql = String.Format("SELECT IdProgramu,Kierunek,Specjalnosc,TrybStudiow,RodzajStudiow FROM Programy WHERE IdProgramu='{0}'",WyszProgBox.SelectedItem);
            MySqlCommand wyszProgKom = new MySqlCommand(wyszProgSql,FormularzLogIn.pol);
            MySqlDataReader czytSzukProg = wyszProgKom.ExecuteReader();
            listaWyszProg.Items.Clear();
            if(czytSzukProg.HasRows)
            {
                while(czytSzukProg.Read())
                {
                    listaWyszProg.Items.Add(String.Format("{0}     {1}       {2}           {3}        {4}",czytSzukProg[0].ToString(), czytSzukProg[1].ToString(), czytSzukProg[2].ToString(), czytSzukProg[3].ToString(), czytSzukProg[4].ToString()));
                }

            }
            czytSzukProg.Close();

        }
        private void EdycjaPracBTN_Click(object sender, EventArgs e)
        {
            EditPrac();

        }

        private void EdytujStudBTN_Click(object sender, EventArgs e)
        {
            EditStud();
        }

        private void EdytujPrzedBTN_Click(object sender, EventArgs e)
        {
            EditPrzed();
        }

        private void EdytujProgBTN_Click(object sender, EventArgs e)
        {
            EditProg();
        }

        private void WyszPrzedBTN_Click(object sender, EventArgs e)
        {
            WyszukajPrzedmiot();
        }

        private void WyszProgBTN_Click(object sender, EventArgs e)
        {
            WyszukajProgram();
        }
    }
}
