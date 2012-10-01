using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DataServiceHttpUnitTests
{
    /// <remarks/>
    [GeneratedCode("System.Xml", "2.0.50727.4927")]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/ado/2007/06/edmx")]
    [XmlRoot(Namespace = "http://schemas.microsoft.com/ado/2007/06/edmx", IsNullable = false)]
    public class Edmx
    {
        /// <remarks/>
        public EdmxDataServices DataServices { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public decimal Version { get; set; }
    }

    /// <remarks/>
    [GeneratedCode("System.Xml", "2.0.50727.4927")]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/ado/2007/06/edmx")]
    public class EdmxDataServices
    {
        /// <remarks/>
        [XmlElement(Namespace = "http://schemas.microsoft.com/ado/2006/04/edm")]
        public Schema Schema { get; set; }
    }

    /// <remarks/>
    [GeneratedCode("System.Xml", "2.0.50727.4927")]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/ado/2006/04/edm")]
    [XmlRoot(Namespace = "http://schemas.microsoft.com/ado/2006/04/edm", IsNullable = false)]
    public class Schema
    {
        /// <remarks/>
        public SchemaEntityContainer EntityContainer { get; set; }

        /// <remarks/>
        [XmlElement("EntityType")]
        public SchemaEntityType[] EntityType { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Namespace { get; set; }
    }

    /// <remarks/>
    [GeneratedCode("System.Xml", "2.0.50727.4927")]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/ado/2006/04/edm")]
    public class SchemaEntityContainer
    {
        /// <remarks/>
        [XmlElement("EntitySet")]
        public SchemaEntityContainerEntitySet[] EntitySet { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Qualified,
            Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata")]
        public bool IsDefaultEntityContainer { get; set; }
    }

    /// <remarks/>
    [GeneratedCode("System.Xml", "2.0.50727.4927")]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/ado/2006/04/edm")]
    public class SchemaEntityContainerEntitySet
    {
        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string EntityType { get; set; }
    }

    /// <remarks/>
    [GeneratedCode("System.Xml", "2.0.50727.4927")]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/ado/2006/04/edm")]
    public class SchemaEntityType
    {
        /// <remarks/>
        [XmlArrayItem("PropertyRef", IsNullable = false)]
        public SchemaEntityTypePropertyRef[] Key { get; set; }

        /// <remarks/>
        [XmlElement("Property")]
        public SchemaEntityTypeProperty[] Property { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [GeneratedCode("System.Xml", "2.0.50727.4927")]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/ado/2006/04/edm")]
    public class SchemaEntityTypePropertyRef
    {
        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [GeneratedCode("System.Xml", "2.0.50727.4927")]
    [DebuggerStepThrough]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/ado/2006/04/edm")]
    public class SchemaEntityTypeProperty
    {
        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public bool Nullable { get; set; }
    }
}