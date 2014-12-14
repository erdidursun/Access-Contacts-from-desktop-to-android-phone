package com.example.contactserver;


import android.support.v7.app.ActionBarActivity;

import android.os.Bundle;
import android.os.StrictMode;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

public class MainActivity extends ActionBarActivity {		
	Server server= null;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);	
		final Button button1=(Button)findViewById(R.id.button1);	
		StrictMode.ThreadPolicy policy = new
				StrictMode.ThreadPolicy.Builder()
		.permitAll().build();
		StrictMode.setThreadPolicy(policy);	
		server= new Server(getContentResolver());
		((TextView)findViewById(R.id.textView1)).setText(server.getLocalIpAddress());	

		button1.setOnClickListener(new OnClickListener() {			
			@Override
			public void onClick(View v) {
				server.execute("1923");				
				button1.setEnabled(false);	

			}


		});

	}
	

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		// Handle action bar item clicks here. The action bar will
		// automatically handle clicks on the Home/Up button, so long
		// as you specify a parent activity in AndroidManifest.xml.
		int id = item.getItemId();
		if (id == R.id.action_settings) {
			return true;
		}
		return super.onOptionsItemSelected(item);
	}
}
