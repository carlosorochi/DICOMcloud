﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DICOMcloud.Messaging;


namespace DICOMcloud.IO
{
    public abstract class ObservableStorageLocation : IStorageLocation
    {
        
        public abstract string ContentType
        {
            get ;
        }

        public abstract string ID
        {
            get ;
        }

        public abstract IMediaId MediaId
        {
            get ;
        }

        public abstract string Metadata
        {
            get ;

            set ;
        }

        public abstract string Name
        {
            get ;
        }

        public abstract long Size
        {
            get ;
        }

        public virtual void Delete ( )
        {
            if ( Exists ( ) )
            {
                long contentLength = Size ;
                
                DoDelete ( ) ;
                
                PublisherSubscriberFactory.Instance.Publish ( this, CreateLocationDeletedEventArgs ( contentLength ) ) ;
            }
        }

        public virtual Stream Download ( )
        {
            var stream = DoDownload ( ) ;
            
            PublisherSubscriberFactory.Instance.Publish ( this, CreateLocationDownloadedEventArgs ( stream ) ) ;

            return stream ;
        }

        public virtual void Download ( Stream stream )
        {
            DoDownload ( stream ) ;

            PublisherSubscriberFactory.Instance.Publish ( this, CreateLocationDownloadedEventArgs ( stream ) ) ;
        }

        public abstract bool Exists ( ) ;

        public virtual Stream GetReadStream ( )
        {
            var stream  = DoGetReadStream ( ) ;

            PublisherSubscriberFactory.Instance.Publish ( this, CreateLocationDownloadedEventArgs ( stream ) ) ;

            return stream ;
        }

        public virtual void Upload ( string fileName )
        {
            DoUpload ( fileName ) ; 

            PublisherSubscriberFactory.Instance.Publish ( this, CreateLocationUploadedEventArgs ( fileName ) ) ;
        }
        
        public virtual void Upload ( byte[] buffer )
        {
            DoUpload ( buffer ) ;

            PublisherSubscriberFactory.Instance.Publish ( this, CreateLocationUploadedEventArgs ( buffer ) ) ;
        }
        
        public virtual void Upload ( Stream stream )
        {
            DoUpload ( stream ) ;

            PublisherSubscriberFactory.Instance.Publish ( this, CreateLocationUploadedEventArgs ( stream ) ) ;
        }

        protected virtual void OnLocationDeletedEventCreated(LocationDeletedMessage args)
        {}

        protected virtual void OnLocationDownloadedEventCreated ( LocationDownloadedMessage args ) 
        {}

        protected virtual void OnLocationUploadedEventCreated ( LocationUploadedMessage args ) 
        {}

        protected virtual LocationDeletedMessage CreateLocationDeletedEventArgs ( long contentLength )
        {
            var args = new LocationDeletedMessage ( this ) ;

            args.ContentLength = contentLength ;

            OnLocationDeletedEventCreated ( args ) ;

            return args ;        
        }

        protected virtual LocationDownloadedMessage CreateLocationDownloadedEventArgs ( Stream stream )
        {
            var args = new LocationDownloadedMessage ( this ) ;

            args.ContentLength = stream.Length ;

            OnLocationDownloadedEventCreated ( args ) ;

            return args ;
        }
        
        protected virtual LocationUploadedMessage CreateLocationUploadedEventArgs ( string fileName )
        {              
            var args = new LocationUploadedMessage ( this ) ;
            
            args.ContentLength = new FileInfo ( fileName ).Length ;
            
            OnLocationUploadedEventCreated ( args ) ;

            return args ;
        }

        protected virtual LocationUploadedMessage CreateLocationUploadedEventArgs ( byte[] buffer )
        {   
            var args = new LocationUploadedMessage ( this ) ;

            args.ContentLength = buffer.LongLength ;

            OnLocationUploadedEventCreated ( args ) ;

            return args ;
        }

        protected virtual LocationUploadedMessage CreateLocationUploadedEventArgs ( Stream stream )
        {              
            var args = new LocationUploadedMessage ( this ) ;

            args.ContentLength = stream.Length ;

            OnLocationUploadedEventCreated ( args ) ;
            
            return args ;
        }

        protected abstract void DoDelete ( );
        protected abstract Stream DoDownload ( );
        protected abstract void DoDownload ( Stream stream );
        protected abstract Stream DoGetReadStream ( );
        protected abstract void DoUpload ( string fileName );
        protected abstract void DoUpload ( byte[] buffer );
        protected abstract void DoUpload ( Stream stream );
    }
}
