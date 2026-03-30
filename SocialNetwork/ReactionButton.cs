using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TweetingPlatform
{
    /// <summary>
    /// Social networking platform-д ашиглагдах reaction товч custom control.
    /// Энэ товч нь:
    /// - emoji
    /// - reaction нэр
    /// - reaction count
    /// зэргийг харуулна.
    /// </summary>
    public class ReactionButton : Control
    {
        /// <summary>
        /// Reaction-ийн текст (жишээ: Like, Love, Haha).
        /// </summary>
        private string reactionText = "Like";

        /// <summary>
        /// Reaction-ийн emoji дүрс.
        /// </summary>
        private string reactionEmoji = "👍";

        /// <summary>
        /// Reaction-ийн тоо.
        /// </summary>
        private int reactionCount = 0;

        /// <summary>
        /// Товч дарагдсан эсэхийг хадгална.
        /// </summary>
        private bool isReacted = false;

        /// <summary>
        /// Товч дээр харагдах reaction текст.
        /// </summary>
        [Category("Custom Properties")]
        public string ReactionText
        {
            get { return reactionText; }
            set
            {
                reactionText = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Товч дээр харагдах emoji.
        /// </summary>
        [Category("Custom Properties")]
        public string ReactionEmoji
        {
            get { return reactionEmoji; }
            set
            {
                reactionEmoji = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Reaction-ийн нийт тоо.
        /// </summary>
        [Category("Custom Properties")]
        public int ReactionCount
        {
            get { return reactionCount; }
            set
            {
                reactionCount = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Reaction өөрчлөгдөх үед дуудагдах event.
        /// </summary>
        public event EventHandler ReactionChanged;

        /// <summary>
        /// ReactionButton-ийн constructor.
        /// </summary>
        public ReactionButton()
        {
            this.Size = new Size(120, 32);
            this.BackColor = Color.WhiteSmoke;
            this.Cursor = Cursors.Hand;
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// Товчийг өөрөө зурж харуулна.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Color borderColor = isReacted ? Color.FromArgb(24, 119, 242) : Color.LightGray;
            Color fillColor = isReacted ? Color.FromArgb(230, 240, 255) : Color.White;
            Color textColor = isReacted ? Color.FromArgb(24, 119, 242) : Color.Black;

            using (SolidBrush bgBrush = new SolidBrush(fillColor))
            {
                g.FillRectangle(bgBrush, 0, 0, this.Width - 1, this.Height - 1);
            }

            using (Pen borderPen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
            }

            using (Font emojiFont = new Font("Segoe UI Emoji", 10, FontStyle.Regular))
            using (SolidBrush emojiBrush = new SolidBrush(textColor))
            {
                g.DrawString(reactionEmoji, emojiFont, emojiBrush, 10, 7);
            }

            using (Font textFont = new Font("Segoe UI", 9, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                string displayText = $"{reactionText} ({reactionCount})";
                g.DrawString(displayText, textFont, textBrush, 35, 8);
            }
        }

        /// <summary>
        /// Mouse click үед event үүсгэнэ.
        /// </summary>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            isReacted = true;
            Invalidate();
            ReactionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}