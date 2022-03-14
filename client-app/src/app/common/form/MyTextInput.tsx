import { useField } from "formik";
import React from "react";
import { Form, Label } from "semantic-ui-react";


interface Props {
    placeholder: string;
    name: string;
    type?: string;
    label?: string;
}
export default function MyInputText(props: Props) {
    const [field, meta] = useField(props.name);
    return (                                //makes this object into bool
        <Form.Field error={meta.touched && !!meta.error}>
            <label>{props.label}</label>
            <input {...field} {...props}></input>
            {meta.touched && meta.error ? (
                <Label basic color="red">{meta.error}</Label>
            ) : null}
        </Form.Field>
    );
}