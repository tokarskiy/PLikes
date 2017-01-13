using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace P_Likes
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private String userToken;
        private String userId;
        const String appId = "5072484";

        public AuthWindow()
        {
            InitializeComponent();
            
            GetToken.LoadCompleted += GetToken_LoadCompleted;
            GetToken.Navigate($@"https://oauth.vk.com/authorize?client_id={appId}&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=friends,wall&response_type=token&v=5.52");
        }

        private void GetToken_LoadCompleted(Object sender, EventArgs e)
        {
            if (GetToken.Source.ToString().Contains("access_token="))
            {
                GetUserToken();
            }
        }

        private void GetUserToken()
        {
            var separators = new Char[]{ '=', '&' };
            String[] split = GetToken.Source.ToString().Split(separators);
            userToken = split[1];
            userId = split[5];
            this.Hide();
        }

        public String UserToken => userToken;
        public String UserId => userId;
    }
}
