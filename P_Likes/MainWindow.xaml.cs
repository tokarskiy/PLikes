using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace P_Likes
{
    public partial class MainWindow : Window
    {
        private String userToken = String.Empty;
        private String userId = String.Empty;
        const String domain = "p_lazarus";

        public MainWindow()
        {
            InitializeComponent();

            var window = new AuthWindow();
            window.ShowDialog();

            userToken = window.UserToken;
            userId = window.UserId;
            VkIdLabel.Content = $"Vk Id: {window.UserId}";
            window.Close();

            // если вход не осуществился
            if (userToken == String.Empty)
            {
                this.Close();
            }
        }

        private void LikeButton_Click(Object sender, RoutedEventArgs e)
        {
            LikePostsAsync();
        }

        private async void LikePostsAsync()
        {
            if (!Int32.TryParse(PostsNumberBox.Text, out Int32 postsNumber))
            {
                MessageBox.Show("Number parsing error.");
                PostsNumberBox.Text = String.Empty;
                return;
            }

            var queryUri = $@"https://api.vk.com/method/wall.get?v=5.52&domain={domain}&count={postsNumber}&extended=1&access_token={userToken}";

            // отправка запроса на сервер и получение объекта ответа
            var request = WebRequest.Create(queryUri);
            JObject jsonData = null;
            using (var response = request.GetResponse())
            {
                // оправить запрос на получение постов на П в формате JSON
                jsonData = JObject.Parse(new StreamReader(response.GetResponseStream()).ReadToEnd());
            }

            // пропарсить JSON и получить все посты на П
            var result =
                from post in jsonData["response"]["items"]
                let id = (Int32)post["id"]
                let owner = (Int32)post["owner_id"]
                select new
                {
                    Id = id,
                    Owner = owner,
                };

            ProgressBar.Maximum = postsNumber;
            ProgressBar.Value = 0;
            LikeButton.IsEnabled = false;

            foreach (var post in result)
            {
                String likeQueryUri = $@"https://api.vk.com/method/likes.add?v=5.52&type=post&owner_id={post.Owner}&item_id={post.Id}&access_token={userToken}";
                request = WebRequest.Create(likeQueryUri) as HttpWebRequest;

                // отправить запрос на лайк   
                using (var resp = await request.GetResponseAsync()) { }
                ProgressBar.Value++;
            }

            this.Close();
        }
    }
}
