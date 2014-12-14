package com.example.contactserver;


public class Contact {

private String id;
private String Name;
private String  phoneNumbers;
public Contact(){}
public Contact(String id, String Name, String  phoneNumbers) {

    this.id = id;
    this.Name = Name;
    this.phoneNumbers = phoneNumbers;

}
public String getId() {
	return id;
}

public void setId(String id) {
	this.id = id;
}

public String getName() {
	return Name;
}

public void setName(String name) {
	Name = name;
}

public String getPhoneNumbers() {
	return phoneNumbers;
}

public void setPhoneNumbers(String phoneNumbers) {
	this.phoneNumbers = phoneNumbers;
}

}