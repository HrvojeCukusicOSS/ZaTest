using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Vježba6
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Vježba6.UplatniceDataSet uplatniceDataSet = ((Vježba6.UplatniceDataSet)(this.FindResource("uplatniceDataSet")));
            // Load data into the table Korisnik. You can modify this code as needed.
            Vježba6.UplatniceDataSetTableAdapters.KorisnikTableAdapter uplatniceDataSetKorisnikTableAdapter = new Vježba6.UplatniceDataSetTableAdapters.KorisnikTableAdapter();
            uplatniceDataSetKorisnikTableAdapter.Fill(uplatniceDataSet.Korisnik);
            System.Windows.Data.CollectionViewSource korisnikViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("korisnikViewSource")));
            korisnikViewSource.View.MoveCurrentToFirst();
            // Load data into the table Izvor. You can modify this code as needed.
            Vježba6.UplatniceDataSetTableAdapters.IzvorTableAdapter uplatniceDataSetIzvorTableAdapter = new Vježba6.UplatniceDataSetTableAdapters.IzvorTableAdapter();
            uplatniceDataSetIzvorTableAdapter.Fill(uplatniceDataSet.Izvor);
            System.Windows.Data.CollectionViewSource izvorViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("izvorViewSource")));
            izvorViewSource.View.MoveCurrentToFirst();
            // Load data into the table Uplata. You can modify this code as needed.
            Vježba6.UplatniceDataSetTableAdapters.UplataTableAdapter uplatniceDataSetUplataTableAdapter = new Vježba6.UplatniceDataSetTableAdapters.UplataTableAdapter();
            uplatniceDataSetUplataTableAdapter.Fill(uplatniceDataSet.Uplata);
            System.Windows.Data.CollectionViewSource uplataViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("uplataViewSource")));
            uplataViewSource.View.MoveCurrentToFirst();
        }

        private void unesi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Regex.IsMatch(imeTextBox.Text, @"^[a-z A-Z]+$") && Regex.IsMatch(adresaTextBox.Text, @"^[a-z A-Z 0-9]+$")
                    && Regex.IsMatch(imeTextBox1.Text, @"^[a-z A-Z]+$") && Regex.IsMatch(adresaTextBox1.Text, @"^[a-z A-Z 0-9]+$"))
                {
                    if (Regex.IsMatch(iBANTextBox.Text, @"^([A-Z]{2})+(\d{19})$") && Regex.IsMatch(iBANTextBox1.Text, @"^([A-Z]{2})+(\d{19})$"))
                    {
                        if (Regex.IsMatch(modelTextBox.Text, @"^(HR)+(\d{1,2})$") && Regex.IsMatch(modelTextBox1.Text, @"^(HR)+(\d{1,2})$"))
                        {
                            if (Regex.IsMatch(pozivniBrojTextBox.Text, @"^((\d*-\d*-\d*)|(\d*-\d*))$") && Regex.IsMatch(pozivniBrojTextBox1.Text, @"^((\d*-\d*-\d*)|(\d*-\d*))$"))
                            {
                                if (datumIzvrsenjaDatePicker.SelectedDate != null)
                                {
                                    if (Regex.IsMatch(iznosTextBox.Text, @"\d+(\.\d{1,2})$"))
                                    {
                                        if (Regex.IsMatch(valutaTextBox.Text, @"[A-Z]{3}"))
                                        {

                                            using (var db = new UplatiKlsaDataContext("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Uplatnice.mdf;Integrated Security=True"))
                                            {
                                                Uplata zaUbacit = new Uplata();
                                                Korisnik platitelj = new Korisnik();
                                                Korisnik primatelj = new Korisnik();
                                                Izvor izvrPlatitelja = new Izvor();
                                                Izvor izvrPrimatelja = new Izvor();

                                                platitelj.Ime = imeTextBox.Text;
                                                platitelj.Adresa = adresaTextBox.Text;
                                                primatelj.Ime = imeTextBox1.Text;
                                                primatelj.Adresa = adresaTextBox1.Text;

                                                if (platitelj != primatelj)
                                                {
                                                    zaUbacit.Korisnik = platitelj;
                                                    zaUbacit.Korisnik1 = primatelj;
                                                }
                                                else
                                                    MessageBox.Show("Unjeli se istu osobu za platitelja i primatelja");

                                                izvrPlatitelja.IBAN = iBANTextBox.Text;
                                                izvrPrimatelja.IBAN = iBANTextBox1.Text;
                                                izvrPlatitelja.Model = modelTextBox.Text;
                                                izvrPrimatelja.Model = modelTextBox1.Text;
                                                izvrPlatitelja.PozivniBroj = pozivniBrojTextBox.Text;
                                                izvrPrimatelja.PozivniBroj = pozivniBrojTextBox1.Text;

                                                if (izvrPlatitelja != izvrPrimatelja)
                                                {
                                                    zaUbacit.Izvor = izvrPlatitelja;
                                                    zaUbacit.Izvor1 = izvrPrimatelja;

                                                }
                                                else
                                                    MessageBox.Show("Unjeli ste isti izvor za platitelja i primatelja");

                                                zaUbacit.SifraNamjene = sifraNamjeneTextBox.Text;
                                                zaUbacit.Opis = opisTextBox.Text;
                                                zaUbacit.DatumIzvrsenja = datumIzvrsenjaDatePicker.SelectedDate.Value.Date;
                                                zaUbacit.Valuta = valutaTextBox.Text;
                                                zaUbacit.Iznos = float.Parse(iznosTextBox.Text);

                                                db.Korisniks.InsertOnSubmit(platitelj);
                                                db.Korisniks.InsertOnSubmit(primatelj);
                                                db.Izvors.InsertOnSubmit(izvrPlatitelja);
                                                db.Izvors.InsertOnSubmit(izvrPrimatelja);

                                                db.Uplatas.InsertOnSubmit(zaUbacit);
                                                try
                                                {
                                                    db.SubmitChanges();
                                                    MessageBox.Show("Dodano u BP");
                                                }
                                                catch
                                                {
                                                    throw new Exception("Nije se dodalo u BP");
                                                }
                                            }
                                        }
                                        else
                                            throw new Exception("Krivi unos valute");
                                    }
                                    else
                                        throw new Exception("Kivi unos iznosa");
                                }
                                else
                                    throw new Exception("Kivi unos datuma");
                            }
                            else
                                throw new Exception("Kivi unos pozivnog broja");
                        }
                        else
                            throw new Exception("Kivi unos modela");
                    }
                    else
                        throw new Exception("Kivi unos IBAN-a");
                }
                else
                    throw new Exception("Kivi unos Korisnika");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
    }
}
