import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link } from "react-router-dom";

import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useForm } from "react-hook-form";
import { RegisterValidation } from "@/lib/validation";


const RegisterForm = () => {
  const form = useForm<z.infer<typeof RegisterValidation>>({
    resolver: zodResolver(RegisterValidation)
  });

  function onSubmit(values: z.infer<typeof RegisterValidation>) {
    console.log(values);
  }

  return (
    <Form {...form}>
      <div className="sm:w-420 flex flex-col justify-center w-full text-foreground">
        {/* <img className="max-w-96" src="/assets/images/logo.jpg" alt="logo" /> */}
        <h2 className="font-outfit text-2xl pt-5 sm:pt-12">
          Create a new account
        </h2>
        <form
          onSubmit={form.handleSubmit(onSubmit)}
          className="flex flex-col gap-5 w-full mt-4"
        >
          <FormField
            control={form.control}
            name="username"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Username</FormLabel>
                <FormControl>
                  <Input type="text" className="shadow-input" {...field} />
                </FormControl>
                <FormDescription>
                  This is your public display name.
                </FormDescription>
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="email"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Email</FormLabel>
                <FormControl>
                  <Input type="email" className="shadow-input" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="password"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Password</FormLabel>
                <FormControl>
                  <Input type="password" className="shadow-input" {...field} />
                </FormControl>
              </FormItem>
            )}
          />
          <Button type="submit" className="shad-button-primary">
            Register
          </Button>
          <p className="text-small-regular text-light-2 text-center mt-2">
            Already have an account?
            <Link to="/login" className="text-primary ml-1 text-cyan-700">
              Log in
            </Link>
          </p>
        </form>
      </div>
    </Form>
  );
};

export default RegisterForm;
