using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegramBot
{
    class Program
    {
        private static ReplyKeyboardMarkup main_Key;
        private static ReplyKeyboardMarkup help_Key;

        static void Main(string[] args)
        {
            main_Key = new ReplyKeyboardMarkup
            {
                Keyboard = new KeyboardButton[][] { new KeyboardButton[] { "راهنما", "ارتباط با ما" }, new KeyboardButton[] { "تنظیمات"} },
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
            help_Key = new ReplyKeyboardMarkup
            {
                Keyboard = new KeyboardButton[][] { new KeyboardButton[] { "گزینه ۱", "گزینه ۲" }, new KeyboardButton[] { "گزینه ۳", "گزینه ۴" }, new KeyboardButton[] { "بازگشت" } },
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            var t = Task.Run(() => RunBot());
            Console.ReadKey();
        }

        static async Task RunBot()
        {
            var bot = new TelegramBotClient("your Bot Token");
            var me = await bot.GetMeAsync();

            if (me != null)
            {
                Console.WriteLine("get bot info succeed\nbot username : {0}", me.Username);
            }
            else
            {
                Console.WriteLine("get bot info failed!");
            }

            int offset = 0;
            int loopCount = 0;

            while (true)
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("loop count: {0}", loopCount);

                var updates = await bot.GetUpdatesAsync(offset: offset);

                Console.WriteLine("update count: {0}", updates.Count());

                foreach (var update in updates)
                {
                    try
                    {
                        string userName;
                        string text;
                        long chatId;

                        if (update.CallbackQuery != null)
                        {
                            userName = update.CallbackQuery.Message.Chat.Username;
                            chatId = update.CallbackQuery.Message.Chat.Id;
                            text = update.CallbackQuery.Message.Text;
                        }
                        else
                        {
                            userName = update.Message.Chat.Username;
                            chatId = update.Message.Chat.Id;
                            text = update.Message.Text;
                        }

                        Console.WriteLine("username: {0}", userName);

                        offset = update.Id + 1;

                        if (update.CallbackQuery != null)
                        {
                            var cData = update.CallbackQuery.Data;
                            if (cData == "btn1 clicked")
                            {
                                await bot.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "خب که چی مثلا؟");
                            }
                        }
                        else if (text == "/start")
                        {
                            await bot.SendTextMessageAsync(chatId: chatId, text: "به ربات ما خوش آمدی", replyMarkup: main_Key);
                        }
                        else if (text == "راهنما")
                        {
                            await bot.SendTextMessageAsync(chatId: chatId, text: "چه کمکی میتونم بکنم؟", replyMarkup: help_Key);
                        }
                        else if (text == "ارتباط با ما")
                        {
                            await bot.SendTextMessageAsync(chatId: chatId, text: "من سجاد هستم\nسازنده این ربات\n‍‍‍‍‍پیشنهادات و انتقاداتون رو به ایدی زیر ارسال کنید\n@Sajjad_Us", replyMarkup: main_Key);
                        }
                        else if (text == "بازگشت")
                        {
                            await bot.SendTextMessageAsync(chatId: chatId, text: "خب حالا چی؟", replyMarkup: main_Key);
                        }
                        else if (text == "تنظیمات")
                        {
                            #region make_buttons
                            InlineKeyboardButton btn1 = new InlineKeyboardButton();
                            #endregion

                            #region edit_buttons_info
                            btn1.Text = "کلیک کن!";
                            btn1.CallbackData = "btn1 clicked";
                            #endregion

                            #region connect_buttons_to_keyboard
                            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[][]
                            {
                            new InlineKeyboardButton[] { btn1 }
                            };
                            #endregion

                            #region make_keyboard
                            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);
                            #endregion

                            await bot.SendTextMessageAsync(chatId: chatId, text: "چی رو میخوای عوض کنی؟؟", replyMarkup: keyboard);
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(chatId: chatId, text: "چی شد؟نفهمیدم\nلطفا از کیبورد استفاده کن");
                        }

                        continue;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                loopCount++;
            }
        }
    }
}
