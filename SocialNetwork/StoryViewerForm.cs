using Social.Core.Entities;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TweetingPlatform
{
    /// <summary>
    /// Story-г бүтэн дэлгэцийн жижиг viewer хэлбэрээр харуулах form.
    /// Дээр нь username, доор нь story content харагдана.
    /// </summary>
    public class StoryViewerForm : Form
    {
        /// <summary>
        /// Одоогоор үзэж буй story объект.
        /// </summary>
        private Story story;

        /// <summary>
        /// StoryViewerForm constructor.
        /// </summary>
        /// <param name="story">Харуулах story</param>
        public StoryViewerForm(Story story)
        {
            this.story = story;

            this.Text = "Story Viewer";
            this.Size = new Size(500, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;

            InitializeStoryUI();
        }

        /// <summary>
        /// Story viewer-ийн UI элементүүдийг үүсгэнэ.
        /// </summary>
        private void InitializeStoryUI()
        {
            Label lblUsername = new Label();
            lblUsername.Text = story.Username;
            lblUsername.ForeColor = Color.White;
            lblUsername.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblUsername.Location = new Point(20, 20);
            lblUsername.AutoSize = true;
            this.Controls.Add(lblUsername);

            Label lblTime = new Label();
            lblTime.Text = story.CreatedAt.ToString("g");
            lblTime.ForeColor = Color.LightGray;
            lblTime.Font = new Font("Segoe UI", 9);
            lblTime.Location = new Point(20, 45);
            lblTime.AutoSize = true;
            this.Controls.Add(lblTime);

            Label lblContent = new Label();
            lblContent.Text = story.Content;
            lblContent.ForeColor = Color.White;
            lblContent.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblContent.TextAlign = ContentAlignment.MiddleCenter;
            lblContent.Size = new Size(420, 400);
            lblContent.Location = new Point(30, 150);
            this.Controls.Add(lblContent);

            Button btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Size = new Size(80, 35);
            btnClose.Location = new Point(390, 20);
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }
    }
}