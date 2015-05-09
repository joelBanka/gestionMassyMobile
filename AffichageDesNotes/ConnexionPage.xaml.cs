using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d’élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkID=390556

namespace AffichageDesNotes
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ConnexionPage : Page
    {
        string reponseEnString;

        private HttpClient httpClient;
        public ConnexionPage()
        {
            this.InitializeComponent();

            httpClient = new HttpClient();
            // Limiter la taille du tampon max pour la réponse si nous ne obtenons pas dépassés
            httpClient.MaxResponseContentBufferSize = 256000;
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
        }

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void btValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // string adresse = "http://localhost/gestionmassy/connexion/getIdStagiaire";
                string adresse = "http://labosio.net/gestionmassy/connexion/getIdStagiaire";
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[] 
                 {
                    new KeyValuePair<string, string>("email", tbIdentifiant.Text),
                    new KeyValuePair<string, string>("pwd", tbMotDePasse.Password)
                });
                VariableGlobale.MailDuStagiaire = tbIdentifiant.Text;
                VariableGlobale.MotPassDuStagiaire = tbMotDePasse.Password;

                HttpResponseMessage laResponse = await httpClient.PostAsync(adresse, content);
                laResponse.EnsureSuccessStatusCode();

                lbReponse.Text = laResponse.StatusCode + " " + laResponse.ReasonPhrase + Environment.NewLine;
                reponseEnString = await laResponse.Content.ReadAsStringAsync();
                if (reponseEnString == "")
                {
                    lbElementRecu.Text = "\n\tEmail ou Mot de passe Incorect";
                }
                else
                {
                    VariableGlobale.NomDuStagiaire = reponseEnString;
                    this.Frame.Navigate(typeof(MainPage));
                }
            }
            catch (HttpRequestException hre)
            {
                lbReponse.Text = hre.ToString();
            }
            catch (Exception ex)
            {
                // Pour le debugging
                lbReponse.Text = ex.ToString();
            }

         //   lbElementRecu.Text = (reponseEnString == "") ? "Email ou Mot de passe Incorect" : reponseEnString;
            if (lbElementRecu.Text == "\n\tEmail ou Mot de passe Incorect")
            {
                lbReponse.Text = "\t\tEssaiyer à nouveau";
            }
        }

        private void btAnnuler_Click(object sender, RoutedEventArgs e)
        {
            tbIdentifiant.Text = "";
            tbMotDePasse.Password = "";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tbIdentifiant.Text = "bankajoel@yahoo.fr";
            
            tbMotDePasse.Password = "banka";
        }


    }
}
