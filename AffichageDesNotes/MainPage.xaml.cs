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

// Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=391641

namespace AffichageDesNotes
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HttpClient httpClient;
        private string serverName;

        public MainPage()
        {
            this.serverName = "localhost";
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            httpClient = new HttpClient();
            // Limiter la taille du tampon max pour la réponse si nous ne obtenons pas dépassés
            httpClient.MaxResponseContentBufferSize = 256000;
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            
        }

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d’événement décrivant la manière dont l’utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: préparer la page pour affichage ici.

            // TODO: si votre application comporte plusieurs pages, assurez-vous que vous
            // gérez le bouton Retour physique en vous inscrivant à l’événement
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed.
            // Si vous utilisez le NavigationHelper fourni par certains modèles,
            // cet événement est géré automatiquement.
        }


        private  void Start_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pour la connexion récupération du Nom en fonction des identifiants
                string reponseEnString2;
                string adresse2 = "http://" + serverName + "/gestionmassy/connexion/getNomStagiaire";
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[] 
                 {
                    new KeyValuePair<string, string>("email", VariableGlobale.MailDuStagiaire),
                    new KeyValuePair<string, string>("pwd", VariableGlobale.MotPassDuStagiaire)
                });
                HttpResponseMessage laResponse2 = await httpClient.PostAsync(adresse2, content);
                laResponse2.EnsureSuccessStatusCode();
                reponseEnString2 = await laResponse2.Content.ReadAsStringAsync();

                // Pour la connexion récupération des notes et ...
                string adresse = "http://" + serverName + "/gestionmassy/evaluations/mobile/" + VariableGlobale.NomDuStagiaire;
                string reponseEnString;
                lbElementRecu.Text = "";
                lbMessageReponse.Text = "En Attente de response ...";

                HttpResponseMessage laResponse = await httpClient.GetAsync(adresse);
                laResponse.EnsureSuccessStatusCode();

                lbMessageReponse.Text = laResponse.StatusCode + " " + laResponse.ReasonPhrase + Environment.NewLine;
                reponseEnString = await laResponse.Content.ReadAsStringAsync();
                reponseEnString = reponseEnString.Replace("<br>", Environment.NewLine); // Insérez de nouvelles lignes
                string[] lignes = reponseEnString.Split('\n');
                string result = "";
                foreach (string ligne in lignes)
                {
                    string[] elements = ligne.Split(';');
                    result += "\n\t"+ elements[3] + "  en  " + elements[1] + "\n\t\t" + elements[2] + "\n";
                }
                lbNom.Text = "Bonjour " + reponseEnString2;
                lbElementRecu.Text = result;

                if (lignes != null)
                {
                    lbMessageReponse.Text = "Voici vos notes ↓. \t\t|Se Déconnecter|";
                }
            }
            catch (HttpRequestException hre)
            {
                lbMessageReponse.Text = hre.ToString();
            }
            catch (Exception ex)
            {
                // Pour le debugging
                lbMessageReponse.Text = ex.ToString();
            }
        }

        private void lbMessageReponse_SelectionChanged(object sender, RoutedEventArgs e)
        {
            
        }

        private void lbMessageReponse_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ConnexionPage));
        }


    }
}
