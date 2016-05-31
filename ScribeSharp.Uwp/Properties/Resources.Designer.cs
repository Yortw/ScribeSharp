﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ScribeSharp.Properties {
    using System;
		using System.Reflection;

	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	// This class was auto-generated by the StronglyTypedResourceBuilder
	// class via a tool like ResGen or Visual Studio.
	// To add or remove a member, edit your .ResX file then rerun ResGen
	// with the /str option, or rebuild your VS project.
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ScribeSharp.Properties.Resources", typeof(Resources).GetTypeInfo().Assembly);
					resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot be less than {1} or more than {2}..
        /// </summary>
        internal static string ArgumentOutOfRangeMessage {
            get {
                return ResourceManager.GetString("ArgumentOutOfRangeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cancelled.
        /// </summary>
        internal static string CancelledOutcome {
            get {
                return ResourceManager.GetString("CancelledOutcome", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Completed.
        /// </summary>
        internal static string CompletedOutcome {
            get {
                return ResourceManager.GetString("CompletedOutcome", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to System date/time changed. New time is {0}.
        /// </summary>
        internal static string DateTimeChangedLogEventText {
            get {
                return ResourceManager.GetString("DateTimeChangedLogEventText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error occurred: {0}.
        /// </summary>
        internal static string ErrorOccurred {
            get {
                return ResourceManager.GetString("ErrorOccurred", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed.
        /// </summary>
        internal static string FailedOutcome {
            get {
                return ResourceManager.GetString("FailedOutcome", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected error in logging system..
        /// </summary>
        internal static string GenericLogExceptionMessage {
            get {
                return ResourceManager.GetString("GenericLogExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error writing log event to output location..
        /// </summary>
        internal static string GenericLogWriterExceptionMessage {
            get {
                return ResourceManager.GetString("GenericLogWriterExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} ({1}) failed; {2}..
        /// </summary>
        internal static string JobFailureEventMessage {
            get {
                return ResourceManager.GetString("JobFailureEventMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} completed..
        /// </summary>
        internal static string JobFinishedEventMessage {
            get {
                return ResourceManager.GetString("JobFinishedEventMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Job ID.
        /// </summary>
        internal static string JobIdPropertyName {
            get {
                return ResourceManager.GetString("JobIdPropertyName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Job Name.
        /// </summary>
        internal static string JobNamePropertyName {
            get {
                return ResourceManager.GetString("JobNamePropertyName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} started..
        /// </summary>
        internal static string JobStartedEventMessage {
            get {
                return ResourceManager.GetString("JobStartedEventMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parent Job ID.
        /// </summary>
        internal static string ParentJobIdPropertyName {
            get {
                return ResourceManager.GetString("ParentJobIdPropertyName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot be empty..
        /// </summary>
        internal static string PropertyCannotBeEmpty {
            get {
                return ResourceManager.GetString("PropertyCannotBeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot be empty or only white space..
        /// </summary>
        internal static string PropertyCannotBeEmptyOrWhitespace {
            get {
                return ResourceManager.GetString("PropertyCannotBeEmptyOrWhitespace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot be null..
        /// </summary>
        internal static string PropertyCannotBeNull {
            get {
                return ResourceManager.GetString("PropertyCannotBeNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Split Event, Part {0}.
        /// </summary>
        internal static string SplitEventEntryPrefix {
            get {
                return ResourceManager.GetString("SplitEventEntryPrefix", resourceCulture);
            }
        }
    }
}
