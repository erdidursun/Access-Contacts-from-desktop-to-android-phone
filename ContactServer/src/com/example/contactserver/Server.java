package com.example.contactserver;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Inet4Address;
import java.net.InetAddress;
import java.net.NetworkInterface;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.SocketException;
import java.util.ArrayList;
import java.util.Enumeration;

import java.util.Stack;

import android.content.ContentResolver;
import android.database.Cursor;
import android.net.Uri;
import android.os.AsyncTask;
import android.provider.ContactsContract;
import android.provider.ContactsContract.PhoneLookup;
import android.provider.ContactsContract.CommonDataKinds.Phone;
import android.provider.ContactsContract.CommonDataKinds.StructuredName;
import android.util.Log;

import com.google.gson.Gson;

public class Server extends AsyncTask<String,String,Boolean> {
	private PrintWriter out;
	private BufferedReader in;
	private ServerSocket serverSocket ;
	private Socket clientSocket;
	private ContentResolver contentResolver;
	private Gson gson = new Gson();
	private char splitChar='$';
	int count=0;
	
	public Server(ContentResolver contentResolver){
		this.contentResolver=contentResolver;
	}
	protected Boolean doInBackground(String... args) {	
		try {			
			START(Integer.parseInt(args[0].toString()));				
			String line = null;
			while(true){
				try {
					line = in.readLine();								
					if(line!=null){							
						if(line.startsWith("getAllContact"))			
							readContacts();	
						else if(line.startsWith("updateContact"))	{	
							String str=line.substring(line.indexOf(splitChar)+1);								
							Contact contact=gson.fromJson(str, Contact.class);
							if(updateContact(contact)){								
								out.write("Update" +splitChar+ contact.getId()+splitChar+contact.getName()+splitChar+ contact.getPhoneNumbers());
								out.flush();
							}
						}
						else if(line.startsWith("deleteContact")){
							String id=line.substring(line.indexOf(splitChar)+1);	
							if(deleteContact(id)){
								out.write("Delete"+ splitChar +id);
								out.flush();
							}

						}
						else if(line.equals("stopServer")){
							serverSocket.close();
							break;
						}
					}


				} catch (Exception e) {
					Log.d("3RD1",e.toString());
				}
			}

			return true;	

		} catch (Exception e) {
			Log.d("3RD1", e.toString());
			return false;
		}


	}
	private void START(int port){
		try {
			serverSocket = new ServerSocket(port);
			clientSocket = serverSocket.accept();
			clientSocket.setSendBufferSize(20000);
			out =new PrintWriter(clientSocket.getOutputStream(), true);
			in = new BufferedReader(new InputStreamReader(clientSocket.getInputStream()));			
		}
		catch(Exception ex){

		}

	}

	public void STOP(){        	 
		try {
			clientSocket.close();
			serverSocket.close();
			out.close();
			in.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	private void readContacts(){
		try {
			Contact tmp=null;
			ContentResolver cr = contentResolver;
			Stack<Contact> contacts= new Stack<Contact>();
			out.write("StartContact");
			out.flush();
			Cursor cur = cr.query(ContactsContract.Contacts.CONTENT_URI,
					null, null, null, null);
			int length=cur.getCount();		

			if (length > 0) {	

				while (cur.moveToNext() ) {

					if (Integer.parseInt(cur.getString(cur.getColumnIndex(ContactsContract.Contacts.HAS_PHONE_NUMBER))) > 0) {

						tmp= new Contact();
						tmp.setId(cur.getString(
								cur.getColumnIndex(ContactsContract.Contacts._ID)));
						tmp.setName( cur.getString(
								cur.getColumnIndex(ContactsContract.Contacts.DISPLAY_NAME)));
						Cursor pCur = cr.query(
								ContactsContract.CommonDataKinds.Phone.CONTENT_URI, 
								null, 
								ContactsContract.CommonDataKinds.Phone.CONTACT_ID +" = ?", 
								new String[]{tmp.getId()}, null);						

						while (pCur.moveToNext()) {

							tmp.setPhoneNumbers(pCur.getString( pCur.getColumnIndex(ContactsContract.CommonDataKinds.Phone.NUMBER)));
							break;
						} 
						pCur.close();
					}
					if(tmp!=null)
						contacts.add(tmp);	
					tmp=null;
					count++;					
				}					
				out.write(gson.toJson(contacts));
				out.flush();	
				try {	
					Thread.sleep(2000);                 //1000 milliseconds is one second.
				} catch(InterruptedException ex) {
					Thread.currentThread().interrupt();
				}
				out.write("EndContact");
				out.flush();
			}	
		}
		catch (Exception e) {

		}


	}

	private boolean updateContact(Contact contact) 
	{
		boolean success = true;
		try
		{      

			if(contact.getName().equals("")&&contact.getPhoneNumbers().equals(""))
			{
				success = false;
			}				           
			else 
			{				

				String where = ContactsContract.Data.CONTACT_ID + " = ? AND " + ContactsContract.Data.MIMETYPE + " = ?"; 

				String[] nameParams = new String[]{contact.getId(), ContactsContract.CommonDataKinds.StructuredName.CONTENT_ITEM_TYPE}; 
				String[] numberParams = new String[]{contact.getId(), ContactsContract.CommonDataKinds.Phone.CONTENT_ITEM_TYPE}; 

				ArrayList<android.content.ContentProviderOperation> ops = new ArrayList<android.content.ContentProviderOperation>();


				if(!contact.getName().equals(""))
				{
					ops.add(android.content.ContentProviderOperation.newUpdate(android.provider.ContactsContract.Data.CONTENT_URI)
							.withSelection(where,nameParams)
							.withValue(StructuredName.DISPLAY_NAME, contact.getName())
							.build());
				}

				if(!contact.getPhoneNumbers().equals(""))
				{

					ops.add(android.content.ContentProviderOperation.newUpdate(android.provider.ContactsContract.Data.CONTENT_URI)
							.withSelection(where,numberParams)
							.withValue(Phone.NUMBER, contact.getPhoneNumbers())
							.build());
				}
				contentResolver.applyBatch(ContactsContract.AUTHORITY, ops);
			}
		}
		catch (Exception e) 
		{
			e.printStackTrace();
			out.write(e.toString());
			out.flush();
			success = false;
		}
		return success;
	}
	private  boolean deleteContact(String Id) {		  

		Cursor cur = contentResolver.query(ContactsContract.Contacts.CONTENT_URI,
				null, null, null, null);
		while (cur.moveToNext()) {
			try{
				if (cur.getString(cur.getColumnIndex(PhoneLookup._ID)).equalsIgnoreCase(Id)) {
					String lookupKey = cur.getString(cur.getColumnIndex(ContactsContract.Contacts.LOOKUP_KEY));
					Uri uri = Uri.withAppendedPath(ContactsContract.Contacts.CONTENT_LOOKUP_URI, lookupKey);
					System.out.println("The uri is " + uri.toString());
					contentResolver.delete(uri, null, null);
					return true;
				}
			}
			catch(Exception e)
			{
				System.out.println(e.getStackTrace());
				return false;
			}
		}
		return true;
	}
	public String getLocalIpAddress() {
		try {
			for (Enumeration<NetworkInterface> en = NetworkInterface.getNetworkInterfaces(); en.hasMoreElements();) {
				NetworkInterface intf = en.nextElement();
				for (Enumeration<InetAddress> enumIpAddr = intf.getInetAddresses(); enumIpAddr.hasMoreElements();) {
					InetAddress inetAddress = enumIpAddr.nextElement();
					if (!inetAddress.isLoopbackAddress() && inetAddress instanceof Inet4Address) {
						return inetAddress.getHostAddress();
					}
				}
			}
		} catch (SocketException ex) {
			ex.printStackTrace();
		}
		return null;
	}
}
