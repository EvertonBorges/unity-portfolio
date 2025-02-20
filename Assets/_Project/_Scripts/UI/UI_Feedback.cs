using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System;

public class UI_Feedback : MonoBehaviour
{

    [SerializeField] private Button _btnSendFeedback;
    [SerializeField] private TMP_InputField _iptName;
    [SerializeField] private TMP_InputField _iptEmail;
    [SerializeField] private TMP_InputField _iptMessage;

    void Awake()
    {
        _btnSendFeedback.onClick.RemoveAllListeners();
        _btnSendFeedback.onClick.AddListener(BTN_SendFeedback);
    }

    private void BTN_SendFeedback()
    {
        var name = _iptName.text;

        if (!ValidateByStringLength(name))
        {
            // TODO Show Error
            return;
        }

        var email = _iptEmail.text;

        if (!ValidateEmail(email))
        {
            // TODO Show Error
            return;
        }

        var message = _iptMessage.text;

        if (!ValidateByStringLength(message))
        {
            // TODO Show Error
            return;
        }

        SendEmail(name, email, message);
    }

    private void SendEmail(string name, string email, string message)
    {
        var smtpClient = new SmtpClient("smtp.gmail.com", 587)
        {
            UseDefaultCredentials = true,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        // add from,to mailaddresses
        MailAddress from = new(email, name);
        MailAddress to = new("evertonsoaresborges@gmail.com", "Everton Borges");
        MailMessage myMail = new(from, to)
        {
            // add ReplyTo
            // MailAddress replyTo = new MailAddress("reply@example.com");
            // myMail.ReplyToList.Add(replyTo);

            // set subject and encoding
            Subject = "Interactive Portfolio - Feedback",
            SubjectEncoding = System.Text.Encoding.UTF8,

            // set body-message and encoding
            Body = "================ <b>FEEDBACK</b> ================<br>",
            BodyEncoding = System.Text.Encoding.UTF8,
            IsBodyHtml = true
        };

        myMail.Body += message;

        try
        {
            smtpClient.Send(myMail);
        }
        catch (SmtpException ex)
        {
            throw new ApplicationException("SmtpException has occured: " + ex.Message);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private bool ValidateByStringLength(string value, int min = 6, int max = 100)
    {
        if (value.IsEmpty())
            return false;

        if (value.Length < min)
            return false;

        if (value.Length > max)
            return false;

        return true;
    }

    private bool ValidateEmail(string value)
    {
        return Regex.IsMatch(value, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }

}
