using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.Generic;

namespace FluentUITest
{
	public sealed partial class MainPage : Page
	{
		private int currentQuestionIndex = 0;
		private Dictionary<string, int> careerScores = new();
		private List<Question> questions;

		public MainPage()
		{
			this.InitializeComponent();
			LoadQuestions();
		}

		// ================= QUESTIONS =================
		private void LoadQuestions()
		{
			questions = new List<Question>
			{
				new Question
				{
					Text = "Which subject do you enjoy the most?",
					Options = new List<Option>
					{
						new Option("Maths", "Data Scientist"),
						new Option("Art & Design", "UI/UX Designer"),
						new Option("Logic & Problem Solving", "Software Developer"),
					}
				},
				new Question
				{
					Text = "Which tool excites you more?",
					Options = new List<Option>
					{
						new Option("Figma / Photoshop", "UI/UX Designer"),
						new Option("Python", "Data Scientist"),
						new Option("C# / Java", "Software Developer"),
					}
				},
				new Question
				{
					Text = "What’s your dream project?",
					Options = new List<Option>
					{
						new Option("A cool looking website", "UI/UX Designer"),
						new Option("A machine learning model", "Data Scientist"),
						new Option("A cross-platform app", "Software Developer"),
					}
				}
			};
		}

		// ================= START QUIZ =================
		private void StartQuizButton_Click(object sender, RoutedEventArgs e)
		{
			StartQuizButton.Visibility = Visibility.Collapsed;

			QuizStack.Visibility = Visibility.Visible;
			QuizStack.Opacity = 1;

			currentQuestionIndex = 0;
			careerScores.Clear();

			ShowQuestion(animated: true);
		}

		// ================= SHOW QUESTION =================
		private void ShowQuestion(bool animated = true)
		{
			if (currentQuestionIndex >= questions.Count)
			{
				ShowResult();
				return;
			}

			var q = questions[currentQuestionIndex];
			QuizQuestion.Text = q.Text;

			OptionsPanel.Children.Clear();

			if (animated)
				AnimateQuestionIn();

			int delay = 0;

			foreach (var opt in q.Options)
			{
				var btn = new Button
				{
					Content = opt.Text,
					Width = 220,
					Padding = new Thickness(20, 10, 20, 10),
					HorizontalAlignment = HorizontalAlignment.Center,
					CornerRadius = new CornerRadius(8),
					Margin = new Thickness(0, 5, 0, 5),
					Opacity = 0
				};

				btn.Click += (s, e) =>
				{
					if (!careerScores.ContainsKey(opt.Career))
						careerScores[opt.Career] = 0;

					careerScores[opt.Career]++;
					currentQuestionIndex++;

					AnimateQuestionOut(() =>
					{
						ShowQuestion(animated: true);
					});
				};

				OptionsPanel.Children.Add(btn);
				AnimateOptionButton(btn, delay);
				delay += 80; // stagger ✨
			}
		}

		// ================= RESULT =================
		private void ShowResult()
		{
			OptionsPanel.Children.Clear();

			string bestCareer = "Developer";
			int maxScore = -1;

			foreach (var career in careerScores)
			{
				if (career.Value > maxScore)
				{
					maxScore = career.Value;
					bestCareer = career.Key;
				}
			}

			QuizQuestion.Text = $"🎉 Your best-fit career is: {bestCareer}!";

			var emoji = new TextBlock
			{
				Text = "🥳🎊🎉",
				FontSize = 42,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0, 16, 0, 0),
				Opacity = 0
			};

			OptionsPanel.Children.Add(emoji);

			AnimateFadeIn(emoji, 0);
		}

		// ================= ANIMATIONS =================

		private void AnimateQuestionIn()
		{
			var sb = new Storyboard();

			var fade = new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = new Duration(System.TimeSpan.FromMilliseconds(350)),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
			};

			Storyboard.SetTarget(fade, QuizQuestion);
			Storyboard.SetTargetProperty(fade, "Opacity");

			sb.Children.Add(fade);
			sb.Begin();
		}

		private void AnimateQuestionOut(System.Action completed)
		{
			var sb = new Storyboard();

			var fade = new DoubleAnimation
			{
				From = 1,
				To = 0,
				Duration = new Duration(System.TimeSpan.FromMilliseconds(200)),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
			};

			fade.Completed += (s, e) => completed?.Invoke();

			Storyboard.SetTarget(fade, QuizQuestion);
			Storyboard.SetTargetProperty(fade, "Opacity");

			sb.Children.Add(fade);
			sb.Begin();
		}

		private void AnimateOptionButton(UIElement element, int delayMs)
		{
			var sb = new Storyboard();

			var fade = new DoubleAnimation
			{
				From = 0,
				To = 1,
				BeginTime = System.TimeSpan.FromMilliseconds(delayMs),
				Duration = new Duration(System.TimeSpan.FromMilliseconds(300)),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
			};

			Storyboard.SetTarget(fade, element);
			Storyboard.SetTargetProperty(fade, "Opacity");

			sb.Children.Add(fade);
			sb.Begin();
		}

		private void AnimateFadeIn(UIElement element, int delayMs)
		{
			var sb = new Storyboard();

			var fade = new DoubleAnimation
			{
				From = 0,
				To = 1,
				BeginTime = System.TimeSpan.FromMilliseconds(delayMs),
				Duration = new Duration(System.TimeSpan.FromMilliseconds(400)),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
			};

			Storyboard.SetTarget(fade, element);
			Storyboard.SetTargetProperty(fade, "Opacity");

			sb.Children.Add(fade);
			sb.Begin();
		}
	}

	// ================= MODELS =================
	public class Question
	{
		public string Text { get; set; }
		public List<Option> Options { get; set; }
	}

	public class Option
	{
		public string Text { get; set; }
		public string Career { get; set; }

		public Option(string text, string career)
		{
			Text = text;
			Career = career;
		}
	}
}