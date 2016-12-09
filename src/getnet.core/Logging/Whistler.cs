using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
using NLog;

namespace getnet
{
    /// <summary>
    /// Logging service
    /// </summary>
    public class Whistler
    {
        #region Private Properties
        /// <summary>
        /// internal log service
        /// </summary>
        private Logger _logger;
        #endregion Private Properties

        #region Constructor

        public bool IsTraining;

        /// <summary>
        /// Constructor
        /// </summary>
        public Whistler([CallerMemberName]string member = "")
        {
            IsTraining = false;

            //add custom layout renderers
            NLog.Config.ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("web_variables", typeof(getnet.logging.WebVariablesRenderer));
            NLog.Config.ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("whistler_mail", typeof(getnet.logging.WhistlerMailRenderer));
            NLog.Config.ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("mail_subject", typeof(getnet.logging.MailSubjectRenderer));
            NLog.Config.ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("mail_determiner", typeof(getnet.logging.MailDeterminerRenderer));
            _logger = LogManager.GetLogger(member);
        }

        #endregion Constructor

        #region Public Functions

        #region info

        public void Info(string message, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Info, _logger.Name, message);
                ev.Properties["type"] = type;

                _logger.Log(ev);
            }
        }

        public void Info(string message, string details, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Info, _logger.Name, message);
                ev.Properties["details"] = details;
                ev.Properties["type"] = type;
                _logger.Log(ev);
            }
        }

        public void Info(string message, string type, int network)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Info, _logger.Name, message);
                ev.Properties["type"] = type;
                ev.Properties["network"] = network;
                _logger.Log(ev);
            }
        }

        public void Info(string message, string details, string type, int network)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Info, _logger.Name, message);
                ev.Properties["details"] = details;
                ev.Properties["type"] = type;
                ev.Properties["network"] = network;
                _logger.Log(ev);
            }
        }

        #endregion info

        #region warn

        public void Warn(string message, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Warn, _logger.Name, message);
                ev.Properties["type"] = type;
                _logger.Log(ev);
            }
        }

        public void Warn(string message, string details, string type)
        {
            LogEventInfo ev = new LogEventInfo(LogLevel.Warn, _logger.Name, message);
            ev.Properties["details"] = details;
            ev.Properties["type"] = type;
            _logger.Log(ev);
        }

        public void Warn(string message, string type, int network)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Warn, _logger.Name, message);
                ev.Properties["type"] = type;
                ev.Properties["network"] = network;
                _logger.Log(ev);
            }
        }

        public void Warn(string message, string details, string type, int network)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Warn, _logger.Name, message);
                ev.Properties["details"] = details;
                ev.Properties["type"] = type;
                ev.Properties["network"] = network;
                _logger.Log(ev);
            }
        }

        #endregion warn

        #region debug

        public void Debug(string message, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Debug, _logger.Name, message);
                ev.Properties["type"] = type;
                _logger.Log(ev);
            }
        }

        public void Debug(string message, string details, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Debug, _logger.Name, message);
                ev.Properties["details"] = details;
                ev.Properties["type"] = type;
                _logger.Log(ev);
            }
        }

        public void Debug(string message, string type, int network)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Debug, _logger.Name, message);
                ev.Properties["type"] = type;
                ev.Properties["network"] = network;
                _logger.Log(ev);
            }
        }

        public void Debug(string message, string details, string type, int network)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Debug, _logger.Name, message);
                ev.Properties["details"] = details;
                ev.Properties["type"] = type;
                ev.Properties["network"] = network;
                _logger.Log(ev);
            }
        }

        #endregion debug

        #region Error

        public void Error(string message, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Error, _logger.Name, message);
                ev.Properties["type"] = type;
                _logger.Log(ev);
            }
        }

        public void Error(string message, string details, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Error, _logger.Name, message);
                ev.Properties["details"] = details;
                ev.Properties["type"] = type;
                _logger.Log(ev);
            }
        }

        //TODO add exception functions
        public void Error(Exception ex, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Error, _logger.Name, ex.Message);
                //ev.Properties["details"] = ExceptionHelper.ParseErrorMessage(ex);
                if (!string.IsNullOrEmpty(type))
                    ev.Properties["type"] = type;
                else
                    ev.Properties["type"] = ex.GetType();
                _logger.Log(ev);
            }
        }

        public void Error(string message, Exception ex, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Error, _logger.Name, message);
                //ev.Properties["details"] = ExceptionHelper.ParseErrorMessage(ex);
                if (!string.IsNullOrEmpty(type))
                    ev.Properties["type"] = type;
                else
                    ev.Properties["type"] = ex.GetType();
                _logger.Log(ev);
            }
        }

        #endregion Error

        #region Fatal

        public void Fatal(string message, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Fatal, _logger.Name, message);
                ev.Properties["type"] = type;
                _logger.Log(ev);
            }
        }

        public void Fatal(string message, string details, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Fatal, _logger.Name, message);
                ev.Properties["details"] = details;
                ev.Properties["type"] = type;
                _logger.Log(ev);
            }
        }

        //TODO add exception functions
        public void Fatal(Exception ex, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Fatal, _logger.Name, ex.Message);
                //ev.Properties["details"] = ExceptionHelper.ParseErrorMessage(ex);
                if (!string.IsNullOrEmpty(type))
                    ev.Properties["type"] = type;
                else
                    ev.Properties["type"] = ex.GetType();

                _logger.Log(ev);
            }
        }

        public void Fatal(string message, Exception ex, string type)
        {
            if (!IsTraining)
            {
                LogEventInfo ev = new LogEventInfo(LogLevel.Fatal, _logger.Name, message);
                //ev.Properties["details"] = ExceptionHelper.ParseErrorMessage(ex);
                if (!string.IsNullOrEmpty(type))
                    ev.Properties["type"] = type;
                else
                    ev.Properties["type"] = ex.GetType();
                _logger.Log(ev);
            }
        }

        #endregion Fatal

        #endregion Public Functions

    }
}