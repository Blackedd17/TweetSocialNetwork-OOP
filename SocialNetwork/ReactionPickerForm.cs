using System;
using System.Drawing;
using System.Windows.Forms;

namespace TweetingPlatform
{
    /// <summary>
    /// Хэрэглэгч reaction сонгох popup form.
    /// Энэ нь Facebook-ийн reaction picker шиг ажиллана.
    /// </summary>
    public class ReactionPickerForm : Form
    {
        /// <summary>
        /// Хэрэглэгчийн сонгосон reaction нэр.
        /// </summary>
        public string SelectedReaction { get; private set; }

        /// <summary>
        /// ReactionPickerForm constructor.
        /// </summary>
        public ReactionPickerForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(330, 70);
            this.BackColor = Color.White;
            this.TopMost = true;
            this.ShowInTaskbar = false;

            BuildUI();
        }

        /// <summary>
        /// Reaction сонголтын товчнуудыг үүсгэнэ.
        /// </summary>
        private void BuildUI()
        {
            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(10);
            panel.BackColor = Color.White;
            this.Controls.Add(panel);

            AddReactionButton(panel, "👍", "Like");
            AddReactionButton(panel, "❤️", "Love");
            AddReactionButton(panel, "😂", "Haha");
            AddReactionButton(panel, "😢", "Sad");
            AddReactionButton(panel, "😡", "Angry");
            AddReactionButton(panel, "🤗", "Care");
        }

        /// <summary>
        /// Нэг reaction товч үүсгэн panel дээр нэмнэ.
        /// </summary>
        /// <param name="parent">Товчийг байрлуулах panel</param>
        /// <param name="emoji">Emoji дүрс</param>
        /// <param name="reactionName">Reaction нэр</param>
        private void AddReactionButton(FlowLayoutPanel parent, string emoji, string reactionName)
        {
            Button btn = new Button();
            btn.Text = emoji;
            btn.Font = new Font("Segoe UI Emoji", 18);
            btn.Size = new Size(45, 45);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.White;
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(240, 242, 245);
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Color.White;
            };

            btn.Click += (s, e) =>
            {
                SelectedReaction = reactionName;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            parent.Controls.Add(btn);
        }
    }
}